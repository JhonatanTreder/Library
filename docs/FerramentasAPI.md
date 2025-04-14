## Ferramentas utilizadas no template de API

**OBS:** Antes de instalar qualquer pacote, favor navegar até a pasta do seu projeto que contêm o arquivo base de compilação (.csproj) <br> <br>

**Exemplo:**

```bash
cd D:/caminho/ate/o/seu/projeto
```

<br>

<hr>

### ASP.NET Core

Documentação base: https://learn.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-8.0 <br> <br>

**JWT Bearer:** <br>
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-8.0 <br>

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.13
```
<br>

**Identity** <br>
https://learn.microsoft.com/pt-br/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio <br>
```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.13
```

<br>

<hr>

### Entity Framework Core

Documentação base: https://learn.microsoft.com/pt-br/ef/ <br> <br>

**EF Core Design**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.13
```

<br>

**EF Core SQL Server**
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13
```

<br>

**EF Core Tools**

```bash
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.13
```

<br>

<hr>

### Swashbucle (Swagger)
Apesar do Swagger ser automaticamente implementado na versão 8.0, estarei mencionando ele aqui caso tenha ocorrido algum tipo de problema. <br>

Documentação base: https://swagger.io/docs/ <br>
```bash
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

<br>
<hr>

**OBS:** Como já mencionado anteriormente, favor navegar até a pasta do arquivo base de compilação (.csproj) para as dependências funcionarem. <br>

**Exemplo:**

```bash
cd D:/caminho/ate/o/seu/projeto
```
