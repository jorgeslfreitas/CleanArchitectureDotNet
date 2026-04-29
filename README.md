# Mostruário API

Este projeto é uma API desenvolvida para fins de estudo, criada para exemplificar a implementação de **Clean Architecture** em uma aplicação .NET.

---

## 🏗️ Arquitetura e Estrutura do Projeto

A solução foi projetada separando as responsabilidades em quatro camadas principais, garantindo desacoplamento, testabilidade e fácil manutenção:

* **`Mostruario.Domain`**: O núcleo do projeto. Contém as regras de negócio intrínsecas, entidades (ex: `Category`, `Product`), exceções de domínio e interfaces de repositório. Não possui comunicação com bibliotecas ou frameworks externos.
* **`Mostruario.Application`**: Responsável por orquestrar a lógica e os fluxos da aplicação. Contém os casos de uso (*Use Cases*), DTOs, classes de mapeamento (Mappers) e validações. Emprega as regras e definições descritas na camada de Domínio.
* **`Mostruario.Infrastructure`**: Camada que implementa as interfaces de acesso a dados e infraestrutura. Lida com o acesso real a bancos de dados, configuração de frameworks externos, repositórios concretos e controles de segurança.
* **`Mostruario.Api`**: Camada de apresentação e ponto de entrada da solução. Disponibiliza a interface REST, expondo os *Controllers* e endpoints para operações como Autenticação, Produtos e Categorias.

Além destas camadas, o módulo **`Mostruario.Application.Tests`** concentra os testes de unidade focados nos casos de uso.

---

## ⚙️ Principais Funcionalidades (Features)

* **Autenticação**: Gestão de rotas seguras e controle de acessos.
* **Produtos e Categorias**: Endpoints que implementam operações CRUD, com suporte a paginação.
* **Testes de Unidade**: Estrutura orientada à validação das regras de negócio.
* **Documentação Técnica**: Guias extensivos encontrados no diretório `docs/`, descrevendo de fluxos de criação de novos CRUDs aos comandos CLI usuais da plataforma.

---

## 🚀 Como Executar

Para iniciar a aplicação localmente:

1. Acesse o diretório da API:
```bash
cd src/Mostruario.Api
```

2. Execute o projeto localmente:
```bash
dotnet run
```

> **Nota:** Para consulta sobre outros comandos, configurações avançadas e fluxos do projeto, por favor, analise a pasta `docs/`.