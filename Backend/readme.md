# O que é necessário para conseguir correr

- postgreSQL v17.6
- dotnet sdk 8.0.415
- entity framework global 8.0.21
- correr `dotnet restore` para instalar os packages do projeto
- criar connection string para fazer a ligação da db do postgresql com o projeto para o ef funcionar
    - dotnet user-secrets init
    - dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=NOME DA DB;Username=USERNAME;Password=PASSWORD"
  
   