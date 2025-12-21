# Introduction

The backend and infrastructure for a prototype full-stack C#, Preact web
application deployed on Azure AKS using modern tools. This project primarily
focuses on architectural design, implementation, and tools that can be leveraged for enterprise-grade applications.

The frontend repository lives here: https://github.com/CodingFlow/rating-app

The http-to-nats proxy lives here:
https://github.com/CodingFlow/http-to-nats-proxy

Tools and frameworks used by backend and infrastructure:

- [Azure AKS](https://azure.microsoft.com/en-us/products/kubernetes-service) -
  Managed Kubernetes service on Azure.
- [Devbox](https://www.jetify.com/devbox) - Ergonomic tool over
  [Nix](https://nixos.org/) to enable easily creating portable, isolated
  development environments.
- [Docker](https://www.docker.com/) - Container tool chain.
- [Preact](https://preactjs.com/) - lightweight, high-performance alternative to
  React.
- [Nats](https://nats.io/) - Distributed, asynchronous messaging system designed
  for performance, scalability, and ease of use.
- [JetStream](https://docs.nats.io/nats-concepts/jetstream) - NATS message
  persistence engine that allows messages to be stored and replayed at a later
  time.
- [k3d](https://k3d.io/stable/) - Local Kubernetes toolchain for local
  development.
- [k6](https://k6.io/) - Performance testing tool.

# Architectural Design

Definitions:

**Conceptual**: High level, not very technical, business domain level.

**Logical**: Technical, agnostic of particular techlibraries and frameworks.

**Physical/Concrete**: More detailed, uses specific technologies.

([Reference](https://medium.com/@nolomokgosi/conceptual-logical-and-physical-design-c24100846931))

## Logical Design

The main idea is to have a service pull architecture: microservices pull from
queues and communcate using an asynchronous, distributed messaging system via
publish/subscribe.

```mermaid
sequenceDiagram

HTTP Client ->> HTTP to async message: HTTP REST request
HTTP to async message -) Queue: async message REST request
API Service ->> Queue: Listen for messages to pull
Queue -->> API Service: Pull async message REST request
API Service -) HTTP to async message: async message REST response
HTTP to async message ->> HTTP Client: HTTP REST response
```

### HTTP To Async Message

Since all services communicate only through the asynchronous messaging system, a
proxy to convert HTTP requests to async messages is needed to service external
HTTP client requests. To the client, the API appears no different than any other
REST API.

### Pull Services

By pulling from a queue, load balancing can be more efficienct and offer better
service quality. Since service instances will only pull more work if they are
able to handle it, no unserviceable requests will be sent to a service instance
that can't handle it.

For example, a traditional round-robin load balancing could send many heavy
requests to the same instance. Eventually, that instance will reach its maximum
request handling capacity much sooner compared to the other instances. Auto
scaling might not even trigger if it is based on the average cpu/memory
utilization across instances. Yet, the at-capacity instance will continue to
receive requests it cannot service. By using a pull system, the less burdened
instances will service the requests instead, ensuring all requests are serviced.

### Asynchronous Communication

By leveraging an asynchronous messaging system with durable message queues,
service quality features, e.g. retries, can be handled separately from the
applications without resorting to more complex solutions such as side car
applications or a service mesh.

Another feature of asynchronous communication is the ability for _different_
service instances to receive request responses. If the original requesting
service becomes unavailable after sending a request, a different instance can
receive and process the response of the request.

## Concrete Design

Following the conceptual design, the concrete design uses specific frameworks
and tools.

```mermaid
sequenceDiagram

Browser (HTTP Client) ->> HTTP to NATS Proxy: HTTP REST request
HTTP to NATS Proxy -) JetStream Stream/Consumer: NATS message REST request
API Service ->> JetStream Stream/Consumer: Listen for messages to pull
JetStream Stream/Consumer -->> API Service: Pull NATS message REST request
API Service -) HTTP to NATS Proxy: NATS message REST response
HTTP to NATS Proxy ->> Browser (HTTP Client): HTTP REST response
```

## Service Design

- Generally follow the ideas and guidelines for Domain Driven Design from the free book **.NET Microservices: Architecture for Containerized .NET Applications** ([view online](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/))
- Focus on opinionated shared libraries, code generators, and tools to enable standardization, reduce boilerplate, and ease improvements across all services.

Services will be composed of the following five layers:

- AppHost
- API
- Application
- Infrastructure
- Domain

Also there is a `Infrastructure.DesignTime` project which is not a runtime layer, but a supporting design time project for the infrastructure layer.

The following diagram shows the layer dependencies:

```mermaid
flowchart TD
  apphost[AppHost]
  api[API]
  app[Application]
  infrastructure[Infrastructure]
  domain[Domain]
  apphost --> api
  apphost --> app
  apphost --> infrastructure
  api --> app
  app --> domain
  infrastructure --> domain
```

Notice that `Domain` depends on no other layer and that `Application` does not directly depend on `Infrastructure`. `Domain` is the final layer and the core of the application. `Application` indirectly receives dependencies from `Infrastructure` through `AppHost` using dependency injection.

### AppHost Layer

The application composition root and host for the entire application. Responsible for:

- Hosting and starting the main application logic.
- Setting up dependency injection.
- Setting up configuration.

A common library has been created to set up the preceding items: `Service.AppHost.Common`.

### API Layer

The frontend of the service, the API layer contains the technical infrastructure to accept and deserialize requests and serialize responses back to consumers. This includes:

- Connecting to a NATS server.
- Dispatching request queries/commands to the right handler in the Application layer.

A common library has been created to set up the preceding items: `Service.Api.Common`.

The API layer will follow an API-first approach instead of a code-first approach for the following reasons:

- There will be code generation opportunities regardless of approach.
- Code-first means creating a specification using code. Code is not the most ideal format for creating declarative specification. AsyncAPI specifications were made for it.

 Code generation is used to create the needed code from the API specification, [AsyncAPI spec](https://www.asyncapi.com). The common source generator, `AsyncApiBindingsGenerator`, will be used across all services' API layers for this purpose. A useful tool for editing and validating in real time AsyncAPI spec files is [AsyncAPI Studio](https://studio.asyncapi.com/).

### Application Layer

Handles queries and commands from the API layer by orchestrating between the domain and infrastructure layers.

The code generator, `AsyncApiApplicationSupportGenerator`, generates:

- The handler interfaces from the AsyncApi spec for each operation defined.
- All types for query, command, query response, command response, and their nested types.

The handler interfaces are derived from a generic interface with types specified for the request and response types that must be handled. In practical terms, it gives a convenient way to create the concrete handler class with the right method types by using [Visual Studio's "Implement interface" code generation quick action](https://learn.microsoft.com/en-us/visualstudio/ide/reference/implement-interface?view=visualstudio).

### Infrastructure Layer

Contains implementations of repositories that implement the interfaces from the domain layer. Repositories provide an abstraction to database-related logic. Repositories use [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) `DBContext`s to interact with the service's database.

### Domain Layer

All business/domain logic resides here following DDD best practices. Aggregates, entities, value objects, as well as repository interfaces reside in this layer. Repository implementations are not present so only the abstractions are exposed to the application layer.

### Infrastructure.DesignTime Project

Contains Entity Framework Core design time support code for the `DBContext`s in the infrastructure layer, such as for generating database migration code and performing migrations.

# Usage

[Install Devbox](https://www.jetify.com/docs/devbox/installing_devbox/). On
Windows, install WSL2 as a prerequisite as mentioned in the
[installation instructions](https://www.jetify.com/docs/devbox/installing_devbox/?install-method=wsl).

Start the Devbox environment (in WSL shell for Windows) at the root of the
project:

```bash
devbox shell
```

> [!NOTE]
> if you encounter an error with docker, "Cannot connect to the Docker daemon at unix:///var/run/docker.sock. Is the docker daemon running?", Then the workaround is to, unfortunately, install Docker on your machine (or in WSL if on Windows). A [GitHub issue](https://github.com/jetify-com/devbox/issues/2485) has been created with DevBox, but no solutions or workarounds have been proposed.

Scripts for creating a local Kubernetes cluster using k3d and to deploy various
components to the cluster are available as shell aliases for convenience:

| Command                       | Description                                                                                                                    |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| `load_config`                 | Load either `dev` (development) or `prod` (production) environment variables to be used for the other deployment commands.     |
| `create-cluster`              | Creates local k3d cluster with local docker registry. Installs [k8sGateway](https://k8sgateway.io/), NATS, and NATS JetStream. |
| `start-cluster`               | Starts k3d cluster if it is stopped.                                                                                           |
| `stop-cluster`                | Stop k3d cluster.                                                                                                              |
| `delete-cluster`              | Deletes the k3d cluster.                                                                                                       |
| `deploy-nack`                 | Apply JetStream kubernetes configuration.                                                                                      |
| `deploy-gateway`              | Apply k8sGateway kubernetes configurations.                                                                                    |
| `deploy-http-to-nats-proxy`   | Build and push to docker registry the docker image for http-to-nats-proxy and deploy via kubernetes configuration.             |
| `deploy_database`             | Installs Cloudnative-pg and apply Postgresql kubernetes configuration                                                          |
| `deploy-service`              | Build and push to docker registry the docker image for rating-service and deploy via kubernetes configuration.                 |
| `deploy-frontend`             | Build and push to docker registry the docker image for the frontend and deploy via kubernetes configuration.                   |
| `port-forward-gateway`        | Port forward the gateway to localhost so the frontend and backend can be accessed for testing.                                 |
| `create-local-nuget-packages` | Create local nuget packages for local libraries used by the service.                                                           |
| `create-database-migration`   | Creates database migration files via Entity Framework Core.                                                                    |
| `update-database` | Executes database migration using database migration files via kubernetes job.                                                             |

Devbox is set up to run `load_config dev` on starting a devbox environment e.g.
via `devbox shell`.

For first time setup, create local nuget packages of service dependencies in this repository, create the cluster, deploy everything, then port forward
for testing:

```bash
create-cluster
deploy-nack
deploy-database
update-database
create-local-nuget-packages
deploy-service
deploy-http-to-nats-proxy
deploy-gateway
deploy-frontend
port-forward-gateway
```

Then send a request to http://localhost:8080/api/ratings to get ratings from the API. Or
access http://localhost:8080/ui in the browser.
