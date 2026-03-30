#!/bin/bash
set -e  

echo "Aguardando SQL Server ficar pronto..."
sleep 15 # não o ideal, mas po aga é isso, arrumar o healthcheck depois

cd /app/src

echo "Restaurando dependências..."
dotnet restore

echo "Buildando aplicação..."
dotnet build

# tenta criar a migration, no momento já deve existir, mas caso não exista, criar.
echo "Criando migration Initial..."
if dotnet ef migrations list | grep -q "Initial"; then
    echo "Migration 'Initial' já existe, pulando..."
else
    dotnet ef migrations add Initial -o Data/Migrations || {
        echo "ERRO: Falha ao criar migrations. Verifique se DbContext existe!"
        exit 1
    }
fi

echo "Atualizando banco de dados..."
dotnet ef database update || {
    echo "ERRO: Falha ao atualizar banco de dados!"
    exit 1
}

echo "Banco de dados atualizado com sucesso!"
echo "Iniciando aplicação..."

dotnet watch run --urls=http://0.0.0.0:5000