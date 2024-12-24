#!/bin/bash

echo 'Welcome to devbox!' > /dev/null

shopt -s expand_aliases

cluster_name=localdevcluster

alias create-cluster='create_cluster'
alias delete-cluster='k3d cluster delete $cluster_name'
alias start-cluster='k3d cluster start $cluster_name'
alias stop-cluster='k3d cluster stop $cluster_name'

alias deploy-service='deploy_service'

create_cluster() {
    k3d cluster create $cluster_name --image rancher/k3s:v1.31.4-k3s1 --registry-create $cluster_name-registry:localhost:5000 &&
    kubectl apply -f https://github.com/kubernetes-sigs/gateway-api/releases/download/v1.2.1/standard-install.yaml &&
    helm repo add gloo https://storage.googleapis.com/solo-public-helm &&
    helm repo update &&
    helm install -n gloo-system gloo-gateway gloo/gloo \
--create-namespace \
--version 1.18.0-beta34 \
-f -<<EOF
discovery:
  enabled: false
gatewayProxies:
  gatewayProxy:
    disabled: true
gloo:
  disableLeaderElection: true
kubeGateway:
  enabled: true
EOF
}

deploy_service() {
    docker build -t rating-service . &&
    docker push localhost:5000/rating-service &&
    kubectl apply -f ./deployment/app.yaml
}