# Sispat API — Sistema de Gerenciamento de Patrimônio (ASP.NET Core 9)

[![.NET](https://img.shields.io/badge/.NET-9-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Arquitetura](https://img.shields.io/badge/Arquitetura-Clean-blueviolet)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Swagger](https://img.shields.io/badge/Documentação-Swagger-green)](/swagger)

**Sispat API** é uma API RESTful moderna e escalável desenvolvida em **.NET 9**, projetada para o **gerenciamento de ativos corporativos** (patrimônio).  
Responsável por autenticação, controle de acesso e persistência de dados, a API segue os princípios da **Clean Architecture**, garantindo **separação de responsabilidades, testabilidade e fácil manutenção**.

---

##  Principais Funcionalidades

-  **Autenticação e Autorização:** Registro e login de usuários com `.NET Identity` e `JWT (Bearer Tokens)`.
-  **Gerenciamento de Ativos (CRUD):** Criação, consulta, atualização e exclusão de ativos (ex: notebooks, monitores etc.).
-  **Gerenciamento de Categorias:** CRUD completo para categorias de ativos (ex: *Eletrônicos*, *Mobiliário*).
-  **Gerenciamento de Localizações:** CRUD para locais físicos dos ativos (ex: *Bloco A - Sala 101*).
-  **Validação de Dados:** Implementada com `FluentValidation`.
-  **Documentação Interativa:** Geração automática via `Swagger (OpenAPI)`.

---

##  Stack Tecnológica

- **Back-end:** ASP.NET Core 9, Entity Framework Core 9  
- **Banco de Dados:** SQLite  
- **Autenticação:** .NET Identity + JWT  
- **Mapeamento:** AutoMapper  
- **Validação:** FluentValidation  
- **Documentação:** Swagger / OpenAPI

---

##  Arquitetura — Clean Architecture

O projeto segue o padrão **Clean Architecture**, garantindo baixo acoplamento e alta coesão entre as camadas:

```
Sispat.API
│
├── Sispat.Domain          → Entidades de negócio e interfaces de repositório
├── Sispat.Application     → Lógica de aplicação, DTOs, validações e serviços
├── Sispat.Infrastructure  → Persistência de dados (EF Core), repositórios e serviços de infraestrutura
└── Sispat.API             → Controllers, configuração e middlewares
```

- **Domain:** Entidades e regras de negócio puras, sem dependências externas.  
- **Application:** Casos de uso, DTOs, validadores e mapeamentos.  
- **Infrastructure:** Implementações concretas de persistência, repositórios e provedores.  
- **API:** Ponto de entrada HTTP, exposição dos endpoints e configuração de dependências.

---

##  Como Executar Localmente

###  Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 **ou** VS Code com extensão C#

---

###  1. Clonar o Repositório

```bash
git clone https://github.com/leogomeslima/SispatProjeto.git
cd SispatProjeto
```

---

###  2. Configurar Variáveis de Ambiente

Abra `src/Sispat.API/appsettings.json` e configure a chave JWT:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=sispat.db"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_MUITO_LONGA_E_SEGURA_AQUI_PARA_SHA256",
    "Issuer": "SispatAPI",
    "Audience": "SispatApp"
  }
}
```

>  **Dica:** A Connection String padrão cria o arquivo `sispat.db` automaticamente na raiz do projeto da API.

---

###  3. Aplicar Migrações do Banco de Dados

Abra a solução `Sispat.sln` no Visual Studio e selecione **Sispat.API** como projeto de inicialização.

No **Console do Gerenciador de Pacotes**:

```bash
Update-Database
```

Isso criará o banco `sispat.db` com as tabelas necessárias.

---

###  4. Executar a Aplicação

Pressione **F5** ou clique em **Run/Play** com `Sispat.API` como projeto de inicialização.

A API será iniciada e abrirá automaticamente o **Swagger UI** no navegador.

---

##  Documentação (Swagger)

A documentação interativa da API está disponível no Swagger UI:

 **URL de acesso:**

https://localhost:<PORTA>/swagger

---

###  Endpoints Principais

####  Autenticação
- **POST** `/api/Auth/register` — Registrar um novo usuário  
- **POST** `/api/Auth/login` — Obter token JWT  

####  Categorias
- **GET** `/api/Categories` — Listar todas as categorias  

####  Ativos
- **POST** `/api/Assets` — Criar novo ativo *(requer autenticação)*  

---

##  Padrões e Boas Práticas

- Segue **Clean Architecture** e princípios **SOLID**  
- Uso de **DTOs** e **AutoMapper** para desacoplamento de camadas  
- **Validação centralizada** com FluentValidation  
- **Unit of Work** e **Repository Pattern** implementados para consistência transacional  
- Configurações via **Dependency Injection (DI)** nativa do .NET

---

##  Contribuições

Contribuições são bem-vindas!  
Sinta-se à vontade para abrir **Issues** ou enviar **Pull Requests** com melhorias, correções ou novas funcionalidades.

---
