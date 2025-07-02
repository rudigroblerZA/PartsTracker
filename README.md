# PartsTracker (C# Version)

A thin vertical slice of a PartsTracker platform designed to track inventory parts flowing through production lines — built using ASP.NET Core, PostgreSQL, and React.

## 📦 Features

- Full CRUD API for parts (`/api/parts`)
- PostgreSQL persistence using Entity Framework Core
- Dockerized backend and database setup
- Health check endpoint (`/health`)
- Basic frontend built in React/Angular (your choice)
- Server-side validation with clear error feedback
- Infrastructure-as-Code via Terraform (plan-only)
- CI via GitHub Actions (build, test, dockerize)

---

## ⚙️ Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/your-username/PartsTracker.git
cd PartsTracker
