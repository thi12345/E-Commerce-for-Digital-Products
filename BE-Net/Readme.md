# ShopApp Backend

Backend API cho he thong e-commerce ban san pham so.

Solution di theo huong Clean Architecture:

- `ShopApp.API`: HTTP API, Swagger, middleware, cau hinh startup.
- `ShopApp.Application`: use case, command, query, validation, DTO mapping.
- `ShopApp.Domain`: entity, value object, domain rule, domain exception.
- `ShopApp.Infrastructure`: EF Core, PostgreSQL, repository, seed data.
- `ShopApp.Tests`: automated tests.

## Dieu Kien Can Co

May clone source ve can co:

- .NET SDK 10.x.
- PostgreSQL, hoac Docker neu muon dung `docker-compose.yml`.
- Ket noi NuGet de restore package.

Project target `net10.0` va co `global.json` pin SDK `10.0.101` voi `rollForward=latestFeature`. Neu may khong co .NET SDK 10 tuong thich thi restore/build/run se fail truoc khi API start.

## Chay Local

1. Clone repository.

2. Dam bao PostgreSQL dang chay.

3. Kiem tra `.env`:

```env
ASPNETCORE_ENVIRONMENT=Development
```

4. Doi connection string trong `.env.dev`:

```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=shopApp;Username=postgres;Password=1234
```

App se doc `.env` truoc, sau do doc `.env.dev` khi `ASPNETCORE_ENVIRONMENT=Development`.

5. Restore va build:

```powershell
dotnet restore ShopApp.slnx
dotnet build ShopApp.slnx --no-restore
```

6. Chay API:

```powershell
dotnet run --project src\ShopApp.API\ShopApp.API.csproj
```

Profile HTTP se tu mo Swagger tai:

```text
http://localhost:5090/swagger
```

Neu terminal hoac IDE khong tu mo browser, mo URL tren bang tay.

## Chay Bang Docker

Repo co san `docker-compose.yml` de chay PostgreSQL va API cung luc.

```powershell
docker compose up --build
```

API trong Docker chay tai:

```text
http://localhost:8080
```

Connection string trong Docker tro den service PostgreSQL:

```text
Host=postgres;Port=5432;Database=shopApp;Username=postgres;Password=1234
```

## Hanh Vi Database Khi Startup

Moi lan API start, app se tu dong chay:

- EF Core migrations.
- Seed data ban dau.

Vi vay DB user trong connection string phai co quyen tao va cap nhat schema.

Neu database da dung schema va da co migration, startup se tiep tuc ma khong apply migration moi.

## Swagger

Swagger chi bat trong moi truong `Development`.

Profile `http` trong `src/ShopApp.API/Properties/launchSettings.json` da cau hinh:

- `launchBrowser: true`
- `launchUrl: swagger`
- `applicationUrl: http://localhost:5090`

API cung redirect `/` sang `/swagger` trong Development. Vi vay mo:

```text
http://localhost:5090
```

se di den Swagger.

## Quy Tac Version Package

Tat ca version NuGet duoc co dinh tap trung trong:

```text
Directory.Packages.props
```

Khong khai bao `Version="..."` truc tiep trong cac file `.csproj`. File `.csproj` chi khai bao package can dung, con version nam o `Directory.Packages.props`.

Repo cung bat lock file bang `RestorePackagesWithLockFile=true`, vi vay moi project co `packages.lock.json`. Khi doi version package, chay:

```powershell
dotnet restore ShopApp.slnx --force-evaluate
```

va commit cac file `packages.lock.json` thay doi.

Version EF Core phai dong bo voi `Npgsql.EntityFrameworkCore.PostgreSQL`.

Hien tai project giu EF Core o `10.0.0` vi `Npgsql.EntityFrameworkCore.PostgreSQL` dang la `10.0.0`.

Cac file quan trong:

- `global.json`: co dinh SDK .NET 10 dung cho repo.
- `Directory.Build.props`: bat central package management va lock file.
- `Directory.Packages.props`: co dinh version package.
- `packages.lock.json`: khoa dependency graph sau restore.

Neu tron version package, project co the build thanh cong nhung van fail luc runtime voi loi dang:

```text
Could not load file or assembly 'Microsoft.EntityFrameworkCore.Relational'
```

## Loi Thuong Gap

### NuGet restore fail

Kiem tra internet, proxy, certificate, hoac firewall. Project can restore package tu NuGet de build on dinh.

### API start duoc nhung request bi loi Windows Event Log

`Program.cs` da cau hinh logging chi ghi ra Console va Debug trong local development, tranh viec ghi Windows Event Log khi user khong co quyen.

### Khong ket noi duoc database

Kiem tra:

- PostgreSQL dang chay.
- Port `5432` truy cap duoc.
- Database name, username, password dung voi connection string trong `.env.dev`.
- DB user co quyen chay migration.

## Test

Chay:

```powershell
dotnet test ShopApp.slnx --no-restore
```
