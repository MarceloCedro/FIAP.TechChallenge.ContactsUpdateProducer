#!/bin/bash

set -e  # Encerra o script em caso de erro

echo "🔐 Aplicando Secret da aplicação..."
kubectl apply -f app-secrets.yaml  # Adiciona o Secret

echo "⚙️ Aplicando ConfigMap da aplicação..."
kubectl apply -f Configmap.yaml  # Aplica o ConfigMap

echo "📦 Aplicando Deployment da aplicação..."
kubectl apply -f Deployment.yaml  # Aplica o Deployment da aplicação

echo "🚪 Aplicando Service da aplicação..."
kubectl apply -f Service.yaml  # Aplica o Service da aplicação

echo "📈 Aplicando Horizontal Pod Autoscaler (HPA)..."
kubectl apply -f hpa.yaml  # Aplica o HPA a partir do arquivo hpa.yam

echo "✅ Tudo aplicado com sucesso!"