#!/bin/bash

echo 'Welcome to devbox!' > /dev/null

shopt -s expand_aliases

cluster_name=localdevcluster

alias create-cluster='create_cluster'
alias delete-cluster='k3d cluster delete $cluster_name'
alias start-cluster='k3d cluster start $cluster_name'
alias stop-cluster='k3d cluster stop $cluster_name'

alias deploy-gateway='kubectl apply -f ./deployment/gateway.yaml'
alias deploy-http-to-nats-proxy='deploy_http_to_nats_proxy'
alias deploy-nack='kubectl apply -f ./deployment/nack.yaml'
alias deploy-service='deploy_service'

alias port-forward-gateway='kubectl port-forward deployment/gloo-proxy-http -n gloo-system 8080:8080'

create_cluster() {
    k3d cluster create $cluster_name --image rancher/k3s:v1.31.4-k3s1 --registry-create $cluster_name-registry:localhost:5000 &&
    kubectl apply -f https://github.com/kubernetes-sigs/gateway-api/releases/download/v1.2.1/standard-install.yaml &&
    kubectl apply -f https://github.com/nats-io/nack/releases/latest/download/crds.yml &&
    helm repo add gloo https://storage.googleapis.com/solo-public-helm &&
    helm repo add nats https://nats-io.github.io/k8s/helm/charts/ &&
    helm repo update &&
    helm install -n gloo-system gloo-gateway gloo/gloo --create-namespace --version 1.17.16 -f ./deployment/gloo-values.yaml &&
    helm install nats nats/nats -f ./deployment/nats-values.yaml &&
    helm install nack nats/nack -f ./deployment/nack-values.yaml
}

deploy_http_to_nats_proxy() {
    docker build -t localhost:5000/http-to-nats-proxy /mnt/c/Users/Blue/source/repos/Git/personal/http-to-nats-proxy &&
    docker push localhost:5000/http-to-nats-proxy &&
    kubectl apply -f ./deployment/http-to-nats-proxy.yaml &&
    kubectl rollout restart deployment http-to-nats-proxy-deployment
}

deploy_service() {
    docker build -t localhost:5000/rating-service . &&
    docker push localhost:5000/rating-service &&
    kubectl apply -f ./deployment/app.yaml
    kubectl rollout restart deployment rating-service-deployment
}