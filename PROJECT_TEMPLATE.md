# CoffeeBeans Project Template

## Muc tieu

Tai lieu nay mo ta template mac dinh cho cac project sau nay neu muon di theo kien truc va luong lam viec cua `CoffeeBeans`.

Muc tieu cua template:

- Monorepo gon, de scan va de mo rong
- Frontend va backend tach ro rang
- Setup local nhanh bang Docker Compose
- API, domain, infrastructure va UI co ranh gioi ro
- De tai su dung cho nhieu bai toan CRUD, dashboard, admin tool, internal app

## Cau truc thu muc chuan

```text
project-root/
|-- frontend/
|-- backend/
|-- docker-compose.yaml
|-- Makefile
|-- README.md
|-- PROJECT_TEMPLATE.md
```

## Nguyen tac kien truc

### 1. Monorepo nhe

- Dat `frontend` va `backend` cung cap o root
- Root chiu trach nhiem orchestration va tai lieu
- Moi app tu quan ly dependency, build, Dockerfile va config rieng

### 2. Frontend mong, ro rang

Stack mac dinh:

- Next.js App Router
- TypeScript
- React Query
- UI library tuy bai toan, uu tien tinh nhat quan

Cau truc uu tien:

```text
frontend/
|-- app/
|   |-- components/
|   |-- layout.tsx
|   |-- page.tsx
|-- lib/
|   |-- api.ts
|   |-- types.ts
|   |-- ...
|-- public/
|-- package.json
|-- Dockerfile
|-- .env
```

Quy uoc:

- `app/page.tsx` va cac route chi dieu phoi UI
- Component tap trung vao render va interaction
- Cac hang so API dat trong `lib/api.ts`
- Kieu du lieu dung chung dat trong `lib/types.ts`
- Data fetching dung React Query
- UI khong chua business logic nang neu co the day xuong backend

### 3. Backend tach lop ro rang

Stack mac dinh:

- ASP.NET Core
- FastEndpoints
- EF Core
- Relational database nhu MySQL hoac PostgreSQL

Cau truc uu tien:

```text
backend/
|-- src/
|   |-- ProjectName.Web/
|   |   |-- Configurations/
|   |   |-- Domain/
|   |   |-- Infrastructure/
|   |   |-- FeatureFolders/
|   |   |-- Program.cs
|-- Directory.Build.props
|-- Directory.Packages.props
|-- Dockerfile
```

Quy uoc:

- `Program.cs` giu mong, chu yeu goi cac extension setup
- `Configurations/` chua wiring cho options, logger, middleware, services
- `Domain/` chua entity, value object, aggregate, quy tac nghiep vu
- `Infrastructure/` chua EF Core, repository, query service, seed data, gateway
- Moi feature nen dong goi theo folder rieng, vi du `FeatureName/List`, `FeatureName/Create`
- Endpoint chi nhan request, validate, goi mediator/service, tra response
- Business logic va query logic khong de lon trong endpoint

### 4. Config va environment nhat quan

- Frontend dung `NEXT_PUBLIC_API_BASE_URL`
- Backend dung `ConnectionStrings__AppDb` trong Docker hoac `ConnectionStrings:AppDb` trong appsettings
- Luon co file mau de chi ro bien moi truong can thiet
- Ten bien moi truong giua local va Docker phai thong nhat

### 5. Database migration va seed

- Migration duoc ap dung khi app backend startup trong moi truong development
- Seed data co the chay luc startup neu database trong
- Cho phep `RecreateOnStartup` chi o development khi can reset nhanh
- Khong dung seed startup theo kieu nguy hiem cho production

### 6. Local dev de khoi dong

Root nen co:

- `docker-compose.yaml` de chay database, backend, frontend
- `Makefile` hoac script tuong duong de lenh ngan gon
- `README.md` mo ta cach chay local trong 2-3 phut

Muc tieu:

- Clone repo
- Chay 1 lenh
- Co duoc app frontend, backend va database

## Tieu chuan code

### Frontend

- TypeScript ro kieu, tranh `any` neu khong can thiet
- State cuc bo de gan component, state server de React Query quan ly
- Component tach nho vua du, dat ten theo vai tro
- Request/response mapping ro rang

### Backend

- Bat nullable
- Treat warnings as errors
- Quan ly version package tap trung neu la .NET solution
- Validate request o endpoint layer
- Domain model khong chua du lieu khong hop le
- Logging va error handling duoc setup tu dau

## Khi nao nen dung template nay

Template nay rat hop voi:

- CRUD app
- Dashboard
- Admin portal
- Internal tools
- Data table + filter + pagination
- He thong co API ro rang va UI web rieng

Khong nhat thiet la lua chon tot nhat neu:

- Project chi co 1 service rat nho
- UI la static site don gian
- Can microservices phuc tap ngay tu dau

## Brief mac dinh cho project moi

Khi tao project moi, co the dua cho Codex prompt nay:

```text
Scaffold project moi theo template CoffeeBeans:
- Monorepo gom frontend + backend + docker-compose + Makefile + README
- Frontend dung Next.js App Router + TypeScript + React Query
- Backend dung ASP.NET Core + FastEndpoints + EF Core + feature folders
- Program.cs mong, setup dua vao extension methods
- Domain, Infrastructure, Configurations tach rieng
- Co Dockerfile cho frontend va backend
- Co env example va huong dan chay local
- Uu tien endpoint mong, business logic dua vao handler/service
```

## Quy uoc hop tac cho cac lan sau

Neu ban noi "lam theo template CoffeeBeans", mac dinh hieu la:

- Giu monorepo root-level
- Tach frontend/backend
- Next.js cho frontend
- ASP.NET Core + FastEndpoints cho backend
- Docker Compose cho local setup
- Kien truc ro folders, endpoint mong, logic tach lop

Neu co thay doi, chi can noi them phan khac template mac dinh.
