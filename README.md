# AutoTTU API

API RESTful desenvolvida em **ASP.NET Core** para gerenciar slots, usuários e operações relacionadas.

---

## 📌 Tecnologias

- .NET 8 / ASP.NET Core
- C# 12
- Entity Framework Core
- SQL Server (ou outro banco)
- Swagger / OpenAPI para documentação de endpoints

---

## 🚀 Funcionalidades

- CRUD completo de Slots, Usuários, CheckIn, Motos (organização das motos em estacionamentos)
- Validação de dados e tratamento de erros
- Documentação automática via Swagger UI
- Suporte a status codes corretos (200, 201, 400, 404, 500)
- Exemplo de endpoints:
  - `GET /slots` → listar todos os slots
  - `GET /slots/{id}` → buscar slot por ID
  - `POST /slots` → criar novo slot
  - `PUT /slots/{id}` → atualizar slot
  - `DELETE /slots/{id}` → apagar slot

---

## ⚙️ Como rodar a API

1. Clone o repositório:
```bash
git clone https://github.com/dudabrigidio/Autottu-API.git
cd Autottu-API
```
2. Restaurar pacotes NuGet:
```bash
dotnet restore
```
3. Executar migrations (se estiver usando Entity Framework Core):
```bash
dotnet ef database update
```
4. Rodar a API:
```bash
dotnet run
```
5. Acessar a documentação Swagger:
Abra no navegador:
```bash
https://localhost:5001/swagger

```

