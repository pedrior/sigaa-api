# SIGAA API (SIGAPI) 📚

O **SIGAPI** é uma API REST **não oficial** para o **[Sistema Integrado de Gestão de Atividades Acadêmicas
(SIGAA)](https://sigaa.ufpb.br/publico/)** da **[Universidade Federal da Paraíba (UFPB)](https://www.ufpb.br/)**,
desenvolvida utilizando técnicas de _web scraping_. Seu objetivo principal é fornecer um acesso moderno, seguro e de
alta performance a dados acadêmicos.

> [!WARNING]
> Este é um projeto independente e não possui qualquer afiliação com a UFPB, a Gerência de Segurança da Informação (GSI)
> ou a Superintendência de Tecnologia da Informação (STI). A utilização desta API é de inteira responsabilidade do
> usuário. As credenciais fornecidas são utilizadas exclusivamente para autenticação no SIGAA em nome do usuário e não
> são armazenadas pela API.

> [!WARNING]
> O projeto está em desenvolvimento inicial e possui poucos recursos acessíveis no momento. Mais funcionalidades serão
> adicionadas futuramente.

## 🚀 Executando

Siga os passos abaixo para executar o projeto em seu ambiente de desenvolvimento local.

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Execução

**1. Clone o repositório:**

```bash
git clone https://github.com/pedrior/sigaa-api.git
cd sigaa-api
```

**2. Defina as configurações de desenvolvimento:**

Execute o comando abaixo na raiz do projeto para iniciar o armazenamento de segredos:

```bash
dotnet user-secrets init --project src/Sigapi/
```

> [!NOTE]
> .NET `user-secrets` é a forma mais segura de gerenciar dados sensíveis em desenvolvimento, evitando que suas
> credenciais sejam expostas no código-fonte ou no `appsettings.json`.

Defina a chave usada para assinar tokens `JWT`:

```bash
dotnet user-secrets set --project src/Sigapi/ "Jwt:Key" "<chave-segura-de-256-bits>"
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
