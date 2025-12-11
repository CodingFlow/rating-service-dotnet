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
alias deploy-nack='kubectl apply -f ./deployment/nack.yaml'
alias deploy-service='deploy_service'
alias deploy-frontend='deploy_frontend'

alias port-forward-gateway='kubectl port-forward deployment/gloo-proxy-http -n gloo-system 8080:8080'

alias create-local-nuget-packages='create_local_nuget_packages'

load_config dev

create_local_nuget_packages() {
    dotnet publish ./Service.Api.Common/
    dotnet publish ./Service.Application.Common/
    dotnet publish ./AsyncApiBindingsGenerator/
    mkdir -p ./local-nuget-feed/ &&
    cp ./Service.Api.Common/Service.Api.Common/bin/release/*.nupkg -t ./local-nuget-feed/ &&
    cp ./Service.Application.Common/Service.Application.Common/bin/release/*.nupkg -t ./local-nuget-feed/ &&
    cp ./AsyncApiBindingsGenerator/AsyncApiBindingsGenerator/bin/release/*.nupkg -t ./local-nuget-feed/
}

create_cluster() {
    k3d cluster create $cluster_name --image rancher/k3s:v1.31.4-k3s1 --registry-create $cluster_name-registry:$docker_registry &&
    install_deploy_infrastructure
}

deploy_gateway() {
    kubectl apply -f ./deployment/gateway.yaml &&
    kubectl apply -f ./deployment/gateway-dev.yaml
}

deploy_http_to_nats_proxy() {
    build_push_docker_image $docker_registry http-to-nats-proxy $http_to_nats_build_directory &&
    apply_rollout http-to-nats-proxy ./deployment/http-to-nats-proxy ./deployment/http-to-nats-proxy-values.yaml http-to-nats-proxy-deployment
}

deploy_service() {
    build_push_docker_image $docker_registry rating-service . &&
    apply_rollout rating-service ./deployment/app ./deployment/app-values.yaml rating-service-deployment
}

deploy_frontend() {
    build_push_docker_image $docker_registry frontend https://github.com/CodingFlow/rating-app.git#main &&
    apply_rollout frontend ./deployment/frontend ./deployment/frontend-values.yaml frontend-deployment
}

apply_rollout() {
    local name=$1
    local deploy_file=$2
    local values_file=$3
    local deployment_name=$4

    local merged_values="$(yq '.docker_registry = strenv(docker_registry_name) | .RATING_SERVICE_URL_BASE = strenv(RATING_SERVICE_URL_BASE)' "$values_file")"

    echo values are:
    echo "$merged_values"

    echo "$merged_values" | helm upgrade --install $name $deploy_file -f - &&
    kubectl rollout restart deployment $deployment_name
}
