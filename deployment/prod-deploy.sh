#!/bin/bash

install_deploy_infrastructure() {
    kubectl apply -f https://github.com/kubernetes-sigs/gateway-api/releases/download/v1.2.1/standard-install.yaml &&
    kubectl apply -f https://github.com/nats-io/nack/releases/latest/download/crds.yml &&
    helm repo add gloo https://storage.googleapis.com/solo-public-helm &&
    helm repo add nats https://nats-io.github.io/k8s/helm/charts/ &&
    helm repo update &&
    helm install -n gloo-system gloo-gateway gloo/gloo --create-namespace --version 1.18.2 -f ./deployment/gloo-values.yaml &&
    helm install nats nats/nats -f ./deployment/nats-values.yaml &&
    helm install nack nats/nack -f ./deployment/nack-values.yaml
}

build_push_docker_image() {
    local registry=$1
    local image_name=$2
    local build_directory=$3
    local dockerfile=$4

    if [[ -n "$4" ]]; then
        local file="-f $dockerfile"
    else
        local file=""
    fi

    docker build -t $registry/$image_name $build_directory $file &&
    docker push $registry/$image_name
}

load_config() {
    local environment=$1
    local config

    if [ "$environment" = "prod" ]; then
        local config="$(yq -n 'load("./deployment/prod-config.yaml")')"

    elif [ "$environment" = "dev" ]; then
        if [ -f "./deployment/local-config.yaml" ]; then
            local config="$(yq -n 'load("./deployment/prod-config.yaml") * load("./deployment/dev-config.yaml") * load("./deployment/local-config.yaml")')"
        else
            local config="$(yq -n 'load("./deployment/prod-config.yaml") * load("./deployment/dev-config.yaml")')"
        fi
    else
        echo "incorrect value for argument. Pass in either 'prod' or 'dev'."
        exit 1
    fi

    export docker_registry="$(echo "$config" | yq '.docker_registry')"
    export docker_registry_name="$(echo "$config" | yq '.docker_registry_name')"
    export RATING_SERVICE_URL_BASE="$(echo "$config" | yq '.RATING_SERVICE_URL_BASE')"
    export http_to_nats_build_directory="$(echo "$config" | yq '.http_to_nats_build_directory')"
    export rating_service_stream_name="$(echo "$config" | yq '.rating_service_stream_name')"
    export rating_service_consumer_name="$(echo "$config" | yq '.rating_service_consumer_name')"
}