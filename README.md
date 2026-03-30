# Tópicos 3 - a1 - Docker Setup

Este projeto roda localmente usando Docker e Docker Compose, com SQL Server (Express ou Azure SQL Edge).

## Pré-requisitos

- [Docker](https://www.docker.com/get-started) instalado e rodando
- [Docker Compose](https://docs.docker.com/compose/) disponível

## Como rodar

1. Abra o Docker e espere ele iniciar.
2. No terminal, na raiz do projeto, execute:

```bash
docker compose up --build
```

3. Aguarde alguns instantes até que:

- O SQL Server esteja pronto  
- As migrations sejam aplicadas  
- O aplicativo seja iniciado  

> Observação: na primeira vez que rodar, isso pode demorar um pouco mais, o container precisa baixar a imagem, restaurar pacotes e criar o banco de dados.

4. Acesse a aplicação no navegador:

```
localhost:5000
```

## Parar a aplicação

- Para parar os containers em execução:

```bash
docker compose down
```

- Para parar os containers em execução e limpar o cache:

```bash
docker compose down -v
```
