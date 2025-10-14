# SIGAA API 📚

Uma API REST **não oficial**, segura e de alto desempenho para o Sistema Integrado de Gestão de Atividades Acadêmicas 
(SIGAA) da Universidade Federal da Paraíba (UFPB).

## ✨ Visão Geral

Esta API atua como um _wrapper_ sobre o SIGAA, extraindo dados através de _web scraping_ para fornecer uma interface 
moderna e estruturada para desenvolvedores. O objetivo é simplificar a integração e a criação de novas aplicações que 
utilizam informações acadêmicas da UFPB.

![SIGAA API](/images/api-docs.png)

> [!WARNING]
> Este é um projeto não oficial e não possui vínculo direto com a UFPB ou a Superintendência de Tecnologia da Informação
> (STI). As informações são obtidas estão sujeitas a alterações conforme o site do SIGAA é atualizado.

## 🌟 Principais Funcionalidades

- **Autenticação**: Gerenciamento de sessão seguro através de tokens JWT.
- **Consulta de Perfil**: Acesso a informações detalhadas do perfil do estudante.
- **Dados Públicos**: Acesso a informações sobre Centros e Departamentos Acadêmicos.

> [!NOTE]
> O projeto está em desenvolvimento inicial e possui poucos recursos acessíveis no momento. Novas funcionalidades estão
> sendo adicionadas.

## 🚀 Executando

Siga os passos abaixo para executar o projeto em seu ambiente de desenvolvimento local.

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

### Execução

**1. Clone o repositório:**

```bash
git clone https://github.com/pedrior/sigaa-api.git
cd sigaa-api
```

**2. Defina as configurações de desenvolvimento:**

Execute o comando abaixo na raiz do projeto para iniciar o armazenamento de segredos:

```bash
dotnet user-secrets init --project src/Sigaa.Api/
```

> [!NOTE]
> .NET `user-secrets` é a forma mais segura de gerenciar dados sensíveis em desenvolvimento, evitando que suas
> credenciais sejam expostas no código-fonte ou no `appsettings.json`.

Defina a chave usada para assinar tokens `JWT`:

```bash
dotnet user-secrets set --project src/Sigaa.Api/ "Jwt:Key" "<chave-segura-de-256-bits>"
```

> [!TIP]
> Você pode obter chaves seguras de 256 bits em [randomkeygen.com](https://randomkeygen.com).

Consulte o arquivo `appsettings.json` para mais detalhes sobre as configurações disponíveis.

**3. Restaure as dependências:**

```bash
dotnet restore
```

**4. Execute:**

```bash
dotnet run --project --launch-profile http
```

A API estará disponível em `http://localhost:5001`.

> [!NOTE]
> Ao acessar a raiz (`/`), você será redirecionado para a documentação da API.

## 📖 Documentação da API

A documentação interativa da API, incluindo endpoints e modelos de requisição e resposta, está disponível em `/docs`.

## 🤝 Como Contribuir

Contribuições são muito bem-vindas! Abra uma **issue** para descrever a sua ideia ou relatar um problema.

## 📄 Licença

Este projeto é distribuído sob a licença [MIT](https://github.com/pedrior/sigaa-api-ufpb/blob/main/LICENSE).
