NET-6-RestAPI-Full-EntityFrame-Core-Crud
Este � um projeto de API RESTful em .NET 6 usando o Entity Framework Core para implementar opera��es CRUD (Criar, Ler, Atualizar, Excluir) em um banco de dados.

Tecnologias
Este projeto utiliza as seguintes tecnologias:

.NET 6
Entity Framework Core 6
ASP.NET Core Web API
Microsoft SQL Server (ou outro banco de dados compat�vel com o Entity Framework Core)
Configura��o
Antes de executar o projeto, certifique-se de ter instalado o .NET 6 SDK em sua m�quina e que seu banco de dados esteja configurado corretamente.

Clone o reposit�rio para sua m�quina local.
Abra o projeto em seu editor de c�digo preferido.
Configure a conex�o do banco de dados no arquivo appsettings.json.
Abra o console do NuGet e execute o comando dotnet restore para instalar as depend�ncias do projeto.
Execute o comando dotnet ef database update para criar as tabelas do banco de dados.
Execute o comando dotnet run para iniciar o servidor de desenvolvimento.
Rotas
O projeto tem as seguintes rotas implementadas:

GET /api/produtos - Retorna uma lista de todos os produtos.
GET /api/produtos/{id} - Retorna um �nico produto pelo ID.
POST /api/produtos - Cria um novo produto.
PUT /api/produtos/{id} - Atualiza um produto existente pelo ID.
DELETE /api/produtos/{id} - Exclui um produto existente pelo ID.
Testes
O projeto tamb�m inclui testes unit�rios para as opera��es CRUD da API.

Para executar os testes, execute o comando dotnet test no console do NuGet.

Contribuindo
Sinta-se � vontade para contribuir com o projeto fazendo um fork e enviando um pull request com suas altera��es.

Licen�a
Este projeto � licenciado sob a MIT License - veja o arquivo LICENSE para mais detalhes.