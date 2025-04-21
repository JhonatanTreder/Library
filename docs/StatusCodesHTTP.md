# Status Codes HTTP Utilizados
Lista contendo todos os status codes utilizados no projeto e para o que cada um serve.

<hr>

## Sucesso (2xx)

### 200 - OK:
**-Explicação:** A requisição foi recebida, compreendida e processada com sucesso. <br>
**-Tipo:** Sucesso <br>
**-Possui corpo?** Sim <br>

<hr>

### 201 - Created
**-Explicação:** Quando um recurso foi criado com sucesso. <br>
**-Tipo:** Sucesso <br>
**-Possui corpo?** Sim (opcional)

<hr>

### 204 - No Content
**-Explicação:** A requisição foi bem-sucedida, mas não há conteúdo para retornar. <br>
**-Tipo:** Sucesso <br>
**-Possui corpo?** Não

<hr>

## Erro no Cliente (4xx)

### 400 - Bad Request
**-Explicação:** A requisição está inválida ou malformada. <br>
**-Tipo:** Erro no cliente <br>
**-Possui corpo?** Sim <br>

<hr>

### 401 - Unauthorized
**-Explicação:** A requisição não foi autorizada. <br>
**-Tipo:** Erro no cliente <br>
**-Possui corpo?** Sim <br>

<hr>

### 404 - Not Found
**-Explicação:** O recurso não foi encontrado. <br>
**-Tipo:** Erro no cliente <br>
**-Possui corpo?** Sim <br>

<hr>

### 409 - Conflict
**-Explicação:** Há um conflito com o estado atual do recurso <br>
**-Tipo:** Erro no cliente <br>
**-Possui corpo?** Sim <br>

<hr>

## Erro no Servidor (5xx)

### 500 - Internal Server Error
**-Explicação:** Erro genérico no servidor, algo inesperado pode ter acontecido. <br>
**-Tipo:** Erro no servidor <br>
**-Possui corpo?** Sim <br>

<hr>

Referência de documentação: https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status
