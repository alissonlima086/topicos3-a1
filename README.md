

# Tópicos 3 - a1 

Trabalho desenvolvido para a matéria de Tópicos em Programação 3 do curso de Sistemas de Informação da UNITINS.
Trabalho desenvolvido por:
- Alisson de Oliveira Lima
- Ana Paula Gomes Miranda

# Docker Setup

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

> Observação: devido a diferença de formatos entre o windows e o linux, pode haver um erro para o docker executar o shell script do Entrypoint, caso ocorra, execute o seguinte comando dentro da raiz do projeto:
```bash
(Get-Content entrypoint.sh -Raw) -replace "`r`n", "`n" | Set-Content entrypoint.sh -NoNewline
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
