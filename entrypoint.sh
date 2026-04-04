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

echo "Limpando migrations antigas..."
MIGRATIONS_DIR="Data/Migrations"
KEEP=1
mapfile -t files < <(ls -t "$MIGRATIONS_DIR"/*.cs 2>/dev/null | grep -v "\.Designer\.cs" | grep -v "ModelSnapshot")
COUNT=${#files[@]}
if [ "$COUNT" -gt "$KEEP" ]; then
    for file in "${files[@]:$KEEP}"; do
        base="${file%.cs}"
        rm -f "$base.cs" "$base.Designer.cs"
        echo "Removida migration: $base"
    done
fi

echo "Criando/atualizando migrations..."
dotnet ef migrations add Auto_$(date +%Y%m%d%H%M%S) --output-dir Data/Migrations 2>/dev/null || echo "Nenhuma mudança no modelo, pulando migration."

echo "Buildando aplicação..."
dotnet build

echo "Iniciando aplicação..."

dotnet run --urls=http://0.0.0.0:5000