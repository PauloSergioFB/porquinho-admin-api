# Porquinho

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)]()
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)]()
[![FIAP](https://img.shields.io/badge/FIAP-ED145B?style=for-the-badge&logoColor=white)]()
[![Oracle](https://img.shields.io/badge/Oracle%20Cloud-F80000?style=for-the-badge&logo=oracle&logoColor=white)]()

O Porquinho é uma aplicação de controle financeiro pessoal desenvolvida para ajudar usuários a compreenderem seus hábitos financeiros e tomarem decisões mais conscientes sobre suas finanças.

O app permite o registro e acompanhamento de receitas, despesas e contas, promovendo uma visão clara sobre a situação financeira do usuário.

> Este repositório contém os arquivos da API Back Office, desenvolvida com .NET.

---

[Arquitetura de Solução](#arquitetura-de-solução) | [Endpoints Principais](#endpoints-principais) | [Setup do Projeto](#setup-do-projeto) | [Stack Tecnológica](#stack-tecnológica) | [Desenvolvedores](#desenvolvedores)

---

## Arquitetura de Solução

![Arquitetura de Solução](./docs/solution-architecture-v2.png)

## Endpoints Principais

A seguir estão listados os principais endpoints disponíveis na API Back Office do projeto Porquinho.

## Autenticação

Para acessar os endpoints protegidos, é necessário autenticar-se na API e enviar o token JWT no cabeçalho das requisições.

```
Authorization: Bearer <seu_token_jwt>
```

O token JWT pode ser obtido através do endpoint de login:

```
POST /auth/login  
{
    "email": <email_cadastrado>,
    "senha": <senha_cadastrada>
}
```

Após autenticar-se com sucesso, a API retornará um token que deverá ser enviado em todas as requisições protegidas.

### Health

```
GET    /health            Verifica se a aplicação está rodando corretamente
GET    /health/database   Verifica a conectividade com o banco de dados
```

### Usuários

```
GET    /users              Lista todos os usuários [PROTEGIDO]  
GET    /users/{id}         Retorna um usuário específico [PROTEGIDO]  
POST   /users              Cadastra um novo usuário  
PUT    /users/{id}         Atualiza informações de um usuário [PROTEGIDO]  
PATCH  /users/{id}         Atualiza parcialmente os dados [PROTEGIDO]  
DELETE /users/{id}         Exclui um usuário [PROTEGIDO]  
```

### Assinaturas [PROTEGIDO]

```
GET    /subscriptions              Lista todas as assinaturas  
GET    /subscriptions/{id}         Retorna uma assinatura específica  
POST   /subscriptions              Cria uma nova assinatura  
PUT    /subscriptions/{id}         Atualiza informações da assinatura  
PATCH  /subscriptions/{id}         Atualiza parcialmente uma assinatura  
DELETE /subscriptions/{id}         Remove uma assinatura  
```

### Status de Assinatura

```
GET    /subscription-status              Lista os status existentes  
GET    /subscription-status/{id}         Retorna um status específico  
POST   /subscription-status              Cria um novo status  
PUT    /subscription-status/{id}         Atualiza um status existente  
PATCH  /subscription-status/{id}         Atualiza parcialmente um status  
DELETE /subscription-status/{id}         Exclui um status  
```

### Tiers de Assinatura

```
GET    /subscription-tiers              Lista os tiers de assinatura  
GET    /subscription-tiers/{id}         Retorna um tier específico  
POST   /subscription-tiers              Cria um novo tier  
PUT    /subscription-tiers/{id}         Atualiza um tier existente  
PATCH  /subscription-tiers/{id}         Atualiza parcialmente um tier  
DELETE /subscription-tiers/{id}         Remove um tier  
```

### Funcionalidades

```
GET    /functionalities              Lista todas as funcionalidades disponíveis  
GET    /functionalities/{id}         Retorna uma funcionalidade específica  
POST   /functionalities              Cria uma nova funcionalidade  
PUT    /functionalities/{id}         Atualiza uma funcionalidade existente  
PATCH  /functionalities/{id}         Atualiza parcialmente uma funcionalidade  
DELETE /functionalities/{id}         Exclui uma funcionalidade  
```

### Transações (NoSQL) [PROTEGIDO]

```
GET    /transactions                Lista todas as transações registradas
GET    /transactions/{id}           Retorna os dados de uma transação específica
POST   /transactions                Registra uma nova transação
PUT    /transactions/{id}           Atualiza completamente uma transação existente
PATCH  /transactions/{id}           Atualiza parcialmente os dados de uma transação
DELETE /transactions/{id}           Remove uma transação do sistema
POST   /transactions/import         Importa as transações do banco de dados relacional para o NoSQL
```

### Logs (NoSQL) [PROTEGIDO]

```
GET    /logs              Lista todos os logs registrados pela aplicação
GET    /logs/{id}         Retorna os dados de um log específico
POST   /logs              Cria um novo registro de log
PUT    /logs/{id}         Atualiza completamente um log existente
PATCH  /logs/{id}         Atualiza parcialmente os dados de um log
DELETE /logs/{id}         Remove um log
```

## Setup do Projeto

### Instalação Local

Antes de iniciar, certifique-se de ter instalado:

- **.NET SDK** (versão 9.0.100 ou superior)
- **Docker**
- **Docker Compose**

#### 1. Clonar Repositório

```bash
# Clonar o repositório
git clone https://github.com/PauloSergioFB/porquinho-admin-api.git

# Acessar o diretório
cd porquinho-admin-api

# Instalar as dependências
dotnet restore
```

#### 2. Configurar o Ambiente

Crie um arquivo .env na raiz do projeto com o seguinte conteúdo (substitua pelos seus próprios dados de conexão, usuário e senha):

```bash
ConnectionStrings__OracleConnection=Data Source=oracle.fiap.com.br:1521/orcl;User Id=<seu_usuario>;Password=<sua_senha>;
ASPNETCORE_ENVIRONMENT=Development
```

#### 3. Iniciar containers Docker

Antes de iniciar a aplicação, é necessário subir os serviços definidos no compose.yml.

```
docker compose up -d
```

#### 4. Iniciar o Projeto

```
dotnet run
```

Após a inicialização, a API estará disponível em: http://localhost:5070  
A documentação interativa (Swagger UI) pode ser acessada em: http://localhost:5070/scalar

### 5. Execução de Testes

Para rodar todos os testes automatizados do projeto:

``` bash
dotnet test
```

## Stack Tecnológica

O projeto utiliza as seguintes tecnologias:

- C# 12 - Linguagem principal utilizada na API.
- .NET 9 - Framework base para construção da aplicação com alto desempenho e suporte multiplataforma.
- Entity Framework Core - ORM utilizado para persistência e mapeamento objeto-relacional.
- Data Annotations - Mapeamento de entidades e validação de dados através de atributos.
- Minimal API - Abordagem leve e direta para definição dos endpoints HTTP.
- Swagger / OpenAPI - Ferramenta para documentação e teste interativo dos endpoints da API.

## Desenvolvedores

[@AntonioDeLuca](https://github.com/antoniodeluca) - Desenvolvedor Backend  
[@EnzoAzevedo](https://github.com/enzoazevedo) - Desenvolvedor Backend  
[@PauloSérgioFB](https://github.com/paulgramador) - Desenvolvedor Mobile
