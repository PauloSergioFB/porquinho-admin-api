# 🐖 Porquinho – Back Office

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green?style=for-the-badge&logo=nuget)
![Oracle](https://img.shields.io/badge/Oracle-Database-red?style=for-the-badge&logo=oracle)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow?style=for-the-badge)

---

## 💡 Sobre o Projeto

O **Porquinho** é uma aplicação de **controle financeiro pessoal** voltada para auxiliar usuários a **compreenderem melhor seus hábitos de consumo** e **tomarem decisões mais conscientes** sobre suas finanças.  

O projeto surge da necessidade de oferecer uma alternativa **simples, acessível e orientada por dados**, para quem busca **organizar receitas, despesas e orçamentos** sem depender de ferramentas complexas.

O **Back Office** representa a **API principal** da aplicação, responsável por centralizar o gerenciamento de **usuários, carteiras, contas, categorias, funcionalidades e assinaturas premium**.

---

## 🛠️ Tecnologias Utilizadas

- **C# 9.0**
- **.NET 9 (Minimal API)**
- **Entity Framework Core**
- **Oracle Database**
- **AutoMapper**
- **FluentValidation**
- **Swagger / OpenAPI**
- **LINQ e Mapeamentos relacionais**

---

## 📌 Endpoints Principais

Abaixo estão listadas as principais rotas organizadas por domínio.  
Cada recurso segue o padrão RESTful, com suporte a métodos `GET`, `GET /{id}`, `POST`, `PUT`, `PATCH` e `DELETE`.

### 🧍 Usuários (`UserEndpoints.cs`)

- `GET /api/v1/users` → Lista todos os usuários  
- `GET /api/v1/users/{id}` → Retorna um usuário específico  
- `POST /api/v1/users` → Cadastra um novo usuário  
- `PUT /api/v1/users/{id}` → Atualiza informações de um usuário  
- `PATCH /api/v1/users/{id}` → Atualiza parcialmente os dados  
- `DELETE /api/v1/users/{id}` → Exclui um usuário  

---

### 💳 Assinaturas (`SubscriptionEndpoints.cs`)

- `GET /api/v1/subscriptions` → Lista todas as assinaturas  
- `GET /api/v1/subscriptions/{id}` → Retorna uma assinatura específica  
- `POST /api/v1/subscriptions` → Cria uma nova assinatura  
- `PUT /api/v1/subscriptions/{id}` → Atualiza informações da assinatura  
- `PATCH /api/v1/subscriptions/{id}` → Atualiza parcialmente  
- `DELETE /api/v1/subscriptions/{id}` → Remove uma assinatura  

---

### 🧾 Status de Assinatura (`SubscriptionStatusEndpoints.cs`)

- `GET /api/v1/subscription-status` → Lista os status existentes  
- `GET /api/v1/subscription-status/{id}` → Retorna um status específico  
- `POST /api/v1/subscription-status` → Cria um novo status  
- `PUT /api/v1/subscription-status/{id}` → Atualiza um status existente  
- `PATCH /api/v1/subscription-status/{id}` → Atualiza parcialmente  
- `DELETE /api/v1/subscription-status/{id}` → Exclui um status  

---

### 🧩 Tiers de Assinatura (`SubscriptionTiersEndpoints.cs`)

- `GET /api/v1/subscription-tiers` → Lista os tiers de assinatura  
- `GET /api/v1/subscription-tiers/{id}` → Retorna um tier específico  
- `POST /api/v1/subscription-tiers` → Cria um novo tier  
- `PUT /api/v1/subscription-tiers/{id}` → Atualiza um tier existente  
- `PATCH /api/v1/subscription-tiers/{id}` → Atualiza parcialmente  
- `DELETE /api/v1/subscription-tiers/{id}` → Remove um tier  

---

### ⚙️ Funcionalidades (`FunctionalitiesEndpoints.cs`)

- `GET /api/v1/functionalities` → Lista todas as funcionalidades disponíveis  
- `GET /api/v1/functionalities/{id}` → Retorna uma funcionalidade específica  
- `POST /api/v1/functionalities` → Cria uma nova funcionalidade  
- `PUT /api/v1/functionalities/{id}` → Atualiza uma funcionalidade existente  
- `PATCH /api/v1/functionalities/{id}` → Atualiza parcialmente  
- `DELETE /api/v1/functionalities/{id}` → Exclui uma funcionalidade  

---

## 🧭 Arquitetura da Solução

📄 **Diagrama da Arquitetura:**  
![Arquitetura da Solução](/docs/solution-architecture.png)

---

## 🚀 Como Executar Localmente

Abaixo está o passo a passo completo para configurar e executar o **Porquinho – Back Office** em ambiente local.

---

### **1. Pré-requisitos**

Antes de iniciar, garanta que você tenha os seguintes itens instalados:

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- [Oracle Database XE](https://www.oracle.com/database/technologies/appdev/xe.html) (ou uma instância remota)
- [Visual Studio Code](https://code.visualstudio.com/), [Visual Studio] ou [JetBrains Rider]

---

### **2. Configuração do Ambiente**

Crie um arquivo `.env` na raiz do projeto:

```properties
ConnectionStrings__OracleConnection=Data Source=oracle.fiap.com.br:1521/orcl;User Id=rm559914;Password=fiap25;
ASPNETCORE_ENVIRONMENT=Development
```

---

### **3. Clonando o Repositório**

```bash
git clone https://github.com/seu-usuario/PorquinhoApi.git
cd porquinho-admin-api
```

---

### **4. Restaurando Dependências**

```bash
dotnet restore
```

---

### **5. Executando o Projeto**

#### Rodar diretamente via CLI
```bash
dotnet run
```

---

### **6. Acessando a Documentação Interativa**

Após iniciar o servidor, acesse o Swagger UI:

> **[http://localhost:5070/scalar](http://localhost:5070/scalar)**

---

## 📐 Especificações Técnicas

- Arquitetura modular com separação em **Data**, **DTOs**, **Endpoints**, **Models**, **Services** e **Filters**  
- Padrão **Repository/Service** com injeção de dependência  
- Persistência via **Entity Framework Core (Code First)**  
- Mapeamento entre entidades e DTOs com **AutoMapper**  
- Validação de entrada via **FluentValidation**  
- Documentação automatizada com **Swagger UI**  

---

## 👥 Contribuidores

| Nome | GitHub | Função |
|------|---------|--------|
| **Antonio de Luca** | [@AntonioDeLuca](https://github.com/Antoniodluca-ads) | Desenvolvimento Backend |
| **Enzo Azevedo** | [@EnzoAzevedo](https://github.com/enzorva) | Desenvolvimento Backend |
| **Paulo Barbosa** | [@PauloSergioFB](https://github.com/PauloSergioFB) | Desenvolvimento Backend e Documentação |
