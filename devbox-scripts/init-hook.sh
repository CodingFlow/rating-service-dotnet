#!/bin/bash

echo 'Welcome to devbox!' > /dev/null

. ./deployment/prod-deploy.sh

shopt -s expand_aliases

cluster_name=localdevcluster

alias create-cluster='create_cluster'
alias delete-cluster='k3d cluster delete $cluster_name'
alias start-cluster='k3d cluster start $cluster_name'
alias stop-cluster='k3d cluster stop $cluster_name'

alias deploy-gateway='deploy_gateway'
alias deploy-http-to-nats-proxy='deploy_http_to_nats_proxy'
alias deploy-nack='deploy_nack'
alias deploy-database='deploy_database'
alias deploy-service='deploy_service'
alias deploy-frontend='deploy_frontend'

alias port-forward-gateway='kubectl port-forward deployment/gloo-proxy-http -n gloo-system 8080:8080'

alias create-local-nuget-packages='create_local_nuget_packages'

alias create-database-migration='create_database_migration'
alias update-database='update_database'

load_config dev

create_local_nuget_packages() {
    dotnet pack -c release -o ./local-nuget-feed ./Service.AppHost.Common/
    dotnet pack -c release -o ./local-nuget-feed ./Service.Application.Common/
    dotnet pack -c release -o ./local-nuget-feed ./Service.Api.Common/
    dotnet pack -c release -o ./local-nuget-feed ./AsyncApiBindingsGenerator/
    dotnet pack -c release -o ./local-nuget-feed ./AsyncApiApplicationSupportGenerator/
}

create_cluster() {
    k3d cluster create $cluster_name --image rancher/k3s:v1.31.4-k3s1 --registry-create $cluster_name-registry:$docker_registry &&
    install_deploy_infrastructure
}

deploy_gateway() {
    kubectl apply -f ./deployment/gateway.yaml &&
    kubectl apply -f ./deployment/gateway-dev.yaml
}

deploy_nack() {
    helm upgrade --install rating-service-queue ./deployment/app-queue -f ./deployment/app-queue-values.yaml
    apply_helm_package rating-service-queue ./deployment/app-queue ./deployment/app-queue-values.yaml rating-service-queue
}

deploy_http_to_nats_proxy() {
    build_push_docker_image $docker_registry http-to-nats-proxy $http_to_nats_build_directory &&
    apply_rollout http-to-nats-proxy ./deployment/http-to-nats-proxy ./deployment/http-to-nats-proxy-values.yaml http-to-nats-proxy-deployment
}

deploy_database() {
    kubectl apply --server-side -f \
        https://raw.githubusercontent.com/cloudnative-pg/cloudnative-pg/release-1.28/releases/cnpg-1.28.0.yaml &&
    kubectl wait pod -l app.kubernetes.io/name=cloudnative-pg -n cnpg-system --for=condition=ready --timeout=10s &&
    kubectl apply -f ./deployment/postgres-cnpg.yaml
}

create_database_migration() {
    kubectl wait pod -l app.kubernetes.io/instance=cluster-example --for=condition=Ready --timeout=10s &&
    dotnet ef migrations add InitialCreate -p ./RatingService.Infrastructure/ --startup-project ./RatingService.Infrastructure.DesignTime/
}

update_database() {
    build_push_docker_image $docker_registry rating-service-database-migration-job . database-migration.Dockerfile &&
    apply_helm_package rating-service-database-migration-job ./deployment/database-migration ./deployment/database-migration-values.yaml
}

deploy_service() {
    build_push_docker_image $docker_registry rating-service . &&
    apply_rollout rating-service ./deployment/app ./deployment/app-values.yaml rating-service-deployment
}

deploy_frontend() {
    build_push_docker_image $docker_registry frontend https://github.com/CodingFlow/rating-app.git#main &&
    apply_rollout frontend ./deployment/frontend ./deployment/frontend-values.yaml frontend-deployment
}

apply_helm_package() {
    local name=$1
    local deploy_file=$2
    local values_file=$3

    local merged_values="$(yq '.docker_registry = strenv(docker_registry_name) | .RATING_SERVICE_URL_BASE = strenv(RATING_SERVICE_URL_BASE) | .rating_service_stream_name = strenv(rating_service_stream_name) | .rating_service_consumer_name = strenv(rating_service_consumer_name)' "$values_file")"

    echo values are:
    echo "$merged_values"

    echo "$merged_values" | helm upgrade --install $name $deploy_file -f -
}

apply_rollout() {
    local name=$1
    local deploy_file=$2
    local values_file=$3
    local deployment_name=$4

    apply_helm_package $name $deploy_file $values_file &&
    kubectl rollout restart deployment $deployment_name
}
