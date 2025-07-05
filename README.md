# PartsTracker (C# Version)

A thin vertical slice of a PartsTracker platform designed to track inventory parts flowing through production lines — built using ASP.NET Core, PostgreSQL, and React.

## 📦 Features

- ✅ Full CRUD API for parts (`/api/parts`)
- ✅ PostgreSQL persistence using Entity Framework Core
- ✅ Dockerized backend and database setup
- ✅ Health check endpoint (`/health`)
- ✅ Basic frontend built in React/Angular (your choice)
- ❌ Server-side validation with clear error feedback
- ❌ Infrastructure-as-Code via Terraform (plan-only)
- ❌ CI via GitHub Actions (build, test, dockerize) -> Sonar

---

## ⚙️ Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/rudigroblerZA/PartsTracker.git
cd PartsTracker
```

### 2. Start with Docker Compose (Recommended)

```bash
docker-compose up --build
```
- This will start the backend API, frontend, and PostgreSQL database.
- The API will be available at `http://localhost:8080` (or as configured).
- The frontend will be available at `http://localhost:8081`.

### 3. Manual Setup (Advanced)

- **Backend:**
  - Navigate to `src/PartsTracker.WebApi` and run:

```bash
dotnet build
dotnet ef database update
dotnet run
```

- **Frontend:**
  - Navigate to `src/PartsTracker.UI/partstracker.ui` and run:
    
```bash
npm install
npm run dev
```

- **Database:**
  - Ensure PostgreSQL is running and connection strings are set in `appsettings.json`.

### 4. Infrastructure as Code (Terraform)

- Navigate to `infrastructure/` and run:

```bash
terraform init
terraform plan
```

- (Apply is not enabled for safety in this demo.)

---

## 🗺️ Architecture Diagram

```
+-------------------+        REST       +---------------------+        EF Core         +-------------------+
|   React Frontend  | <---------------->|   .NET Web API      | <--------------------> |   PostgreSQL DB   |
| (Vite, src/Parts  |                   | (src/PartsTracker.  |                        | (Docker, local or |
| Tracker.UI/...)   |                   | WebApi)             |                        | cloud)            |
+-------------------+                   +---------------------+                        +-------------------+
```

- All components are containerized and orchestrated via Docker Compose for local development.
- Infrastructure can be provisioned in the cloud using Terraform scripts.

---

## 💡 Rationale

- **React** was chosen for the frontend due to its popularity, component model, and rapid development capabilities.
- **.NET Core** provides a robust, scalable, and type-safe backend with strong support for REST APIs and database integration.
- **PostgreSQL** is a reliable, open-source relational database with strong community support.
- **Docker Compose** enables easy local orchestration and onboarding.
- **Terraform** allows for reproducible, version-controlled infrastructure provisioning.
- The architecture is designed for simplicity and clarity, but can be extended for production use (e.g., with managed cloud services, CI/CD, and advanced monitoring/security).