# Biblioteca virtual - Back-end
Projeto WEB para um sistema de uma biblioteca virtual.
## Descrição
Este repositório contém a API RESTful de um sistema de biblioteca virtual desenvolvida em **C# com ASP.NET Core**. 
O sistema permite o gerenciamento de livros, usuários e empréstimos, com autenticação/autorização baseada em tokens JWT. 
A parte do front-end será implementada futuramente utilizando Angular ou React.
### Objetivo
O objetivo desse projeto é otimizar o processo de fluxo/gerenciamento dos recursos existentes em uma biblioteca (como gerenciamento de livros, usuários, etc),
dando ênfase na facilidade de acesso e na otimização desses processos por se tratar de ser uma aplicação WEB.

## Ferramentas Utilizadas e Instruções de Instalação

**API:** [Ferramentas da API](./docs/FerramentasAPI.md) <br>
**ApiUnitTests** [Ferramentas do projeto de testes](./docs/FerramentasTestAPI.md) <br>

**OBS:** Para melhor compatibilidade entre as dependências, favor utilizar as versões compatíveis com o .NET 8.0 

## Instrução de instalação
### Pré-requisitos

**.NET 8.0 SDK** <br>
https://dotnet.microsoft.com/pt-br/download/dotnet/8.0 <br>

**SQL Server** <br>
https://www.microsoft.com/pt-br/sql-server/sql-server-downloads <br>

**IDE/editor de código Compatível** <br>

#### Exemplos: <br>
-Visual Studio (IDE): https://visualstudio.microsoft.com/pt-br/vs/community/ <br>
-Visual Studio Code (editor de código): https://code.visualstudio.com/download <br>

## Códigos de Status HTTP utilizados
É de grande importância documentar de forma clara e bem escrita os Status Code usados no projeto, principalmente pensando no Front-End que consumirá os métodos HTTP da API.
Sendo assim essa sessão está dividida em duas partes: <br>

**1. Status HTTP utilizados:** [StatusCodeHTTP](./docs/StatusCodesHTTP.md) <br>
**2. Retorno de cada método nos controladores:** [MethodsReturn](./docs/controllers-return.md)<br>

A primeira parte se diz respeito a todos cada Status Code utilizados no projeto, como por exemplo, Ok, NotFound, BadRequest, etc.
A segunda parte fala de cada Status Code que os métodos podem retornar em cada controlador. 

## Como Contribuir
A sessão de contribuição desse projeto está no arquivo [CONTRIBUTING.md](CONTRIBUTING.md).
Sua contribuição será muito bem vinda para o desenvolvimento desse sistema e de análises futuras para aqueles que desejam apenas estudar!

## Informações de Contato

**Email:** Jhonatantreder11@gmail.com <br>
**Linkedin:** https://www.linkedin.com/in/jhonatan-treder-40777827a/ <br>
**Discord:** hypexsz <br>
