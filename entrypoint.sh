#!/bin/bash
set -e  

echo "Aguardando SQL Server ficar pronto..."
sleep 15 # não o ideal, mas por agora funciona, arrumar o healthcheck depois

cd /app/src

echo "Restaurando dependências..."
dotnet restore

echo "Buildando aplicação..."
dotnet build

echo "Atualizando banco de dados..."
dotnet ef database update || {
    echo "ERRO: Falha ao atualizar banco de dados!"
    exit 1
}

echo "Banco de dados atualizado com sucesso!"
echo "Iniciando aplicação..."

dotnet watch run --urls=http://0.0.0.0:5000