#!/bin/bash
set -e  

echo "Aguardando SQL Server ficar pronto..."

until /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U SA -P "$SA_PASSWORD" -Q "SELECT 1" -C > /dev/null 2>&1; do
    echo "SQL Server ainda não está pronto."
    sleep 5
done

echo "SQL Server pronto."

cd /app/src

echo "Restaurando dependências..."
dotnet restore

echo "Limpando todas as migrations..."
MIGRATIONS_DIR="Data/Migrations"
find "$MIGRATIONS_DIR" -name "*.cs" -delete 2>/dev/null || true

echo "Criando/atualizando migrations..."
dotnet ef migrations add Auto_$(date +%Y%m%d%H%M%S) --output-dir Data/Migrations 2>/dev/null || echo "Nenhuma mudança no modelo, pulando migration."

echo "Buildando aplicação..."
dotnet build

echo "Iniciando aplicação..."

dotnet run --urls=http://0.0.0.0:5000