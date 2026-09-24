<div align="center">

# 🏛️ University Management System Microservice
### Enterprise Distributed Academic & Campus Governance Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Microservices](https://img.shields.io/badge/Architecture-Microservices-blue?style=for-the-badge&logo=diagram-project&logoColor=white)](#-system-architecture)
[![Docker Ready](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellowgreen?style=for-the-badge)](LICENSE)
[![Author](https://img.shields.io/badge/Author-Omar%20Alfarouk-orange?style=for-the-badge&logo=github&logoColor=white)](https://github.com/OmarAlfar0uk)

<p align="center">
  <a href="#-key-features">Key Features</a> •
  <a href="#-system-architecture">System Architecture</a> •
  <a href="#-microservices-catalog">Microservices Catalog</a> •
  <a href="#-tech-stack">Tech Stack</a> •
  <a href="#-getting-started">Getting Started</a> •
  <a href="#-author">Author</a>
</p>

</div>

---

## 📌 Executive Overview

**University Management System Microservice** is a production-grade, distributed campus management solution built to streamline higher education workflows. Decoupled into specialized microservices, the system handles student admissions, academic curricula, faculty assignments, attendance monitoring, examination grading, and automated notification services with high availability.

> [!NOTE]
> Designed using **Domain-Driven Design (DDD)**, **Clean Architecture**, and an **API Gateway pattern**, ensuring independent service deployability, database-per-service isolation, and robust cross-service communication.

---

## ✨ Key Features

| ⚡ Feature | 💡 Description | 🛠 Engineering Detail |
|---|---|---|
| **🚪 API Gateway Entrypoint** | Centralized reverse proxy for client requests | Single endpoint routing, header normalization, and rate limiting |
| **🎓 Academic & Curriculum Svc** | Course catalogs, credit hour validation, and prerequisites | Domain validation logic ensuring prerequisites before enrollment |
| **📋 Attendance Tracking** | Real-time lecture attendance logging and absence triggers | High-throughput tracking with automated warnings |
| **📝 Examination & Grading** | Exam scheduling, question banks, and automated GPA calculation | Concurrency-safe grading engine with audit history |
| **📬 Distributed Notification Svc** | Asynchronous transactional email dispatch | Decoupled notification pipeline with background queueing |
| **🔐 Role-Based Access Control** | Distinct capabilities for Students, Professors, and University Admins | Stateless JWT token validation across gateway and services |

---

## 🏛 System Architecture

```mermaid
flowchart TD
    subgraph Clients["🖥️ Consumer Layer"]
        WebPortal["💻 University Web Portal"]
        MobileApp["📱 Student & Faculty Mobile App"]
    end

    subgraph Gateway["🚪 Edge Routing"]
        APIGateway["API Gateway / Reverse Proxy<br/>• Path Routing<br/>• Token Authentication<br/>• SSL Termination"]
    end

    subgraph Services["⚙️ Academic Microservices"]
        AcademicSvc["🎓 Academic Service<br/>(Curriculums, Courses, Credits)"]
        AttendanceSvc["📋 Attendance Service<br/>(Absence Tracking, Alerts)"]
        ExamSvc["📝 Exam Service<br/>(Grading, GPA Engine)"]
        EmailSvc["📧 Email Service<br/>(Async Notifications, Reports)"]
    end

    subgraph DataTier["🗄️ Database Per Service"]
        DB1[("Academic DB")]
        DB2[("Attendance DB")]
        DB3[("Exam DB")]
    end

    Clients --> Gateway
    Gateway --> AcademicSvc
    Gateway --> AttendanceSvc
    Gateway --> ExamSvc
    Gateway --> EmailSvc

    AcademicSvc --> DB1
    AttendanceSvc --> DB2
    ExamSvc --> DB3
    ExamSvc -.-> EmailSvc
```

---

## 📦 Microservices Catalog

| Service | Responsibility | Port | Primary Endpoints |
|---|---|---|---|
| **API Gateway** | Request dispatching, reverse proxy, routing | `:5000` | `/api/gateway/*` |
| **AcademicService** | Degrees, courses, semester catalogs, departments | `:5001` | `/api/academic/courses`, `/api/academic/departments` |
| **AttendanceService** | Class attendance, student absence ratios | `:5002` | `/api/attendance/record`, `/api/attendance/summary` |
| **ExamService** | Exam configurations, student submissions, grades | `:5003` | `/api/exams/schedule`, `/api/exams/grades` |
| **EmailService** | Templated alerts, grade release emails | `:5004` | `/api/email/dispatch` |

---

## ⚡ Tech Stack

| Category | Technology | Purpose |
|---|---|---|
| **Core Framework** | ![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=flat-square&logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23_12-239120?style=flat-square&logo=csharp&logoColor=white) | Distributed microservices runtime |
| **Gateway & Routing** | ![Reverse Proxy](https://img.shields.io/badge/API_Gateway-Reverse_Proxy-blue?style=flat-square) | Centralized routing and cross-cutting security |
| **Data & Persistence** | ![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white) ![SQL Server](https://img.shields.io/badge/MS_SQL_Server-CC292B?style=flat-square&logo=microsoftsqlserver&logoColor=white) | Database-per-service relational persistence |
| **Security** | ![JWT](https://img.shields.io/badge/JWT-Bearer_Tokens-black?style=flat-square&logo=jsonwebtokens&logoColor=white) | Inter-service and client authentication |
| **Containers** | ![Docker](https://img.shields.io/badge/Docker-Containers-2496ED?style=flat-square&logo=docker&logoColor=white) | Containerized development and deployment |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) or local SQL Server instance

### Setup & Execution

1. **Clone the repository:**
   ```bash
   git clone https://github.com/OmarAlfar0uk/UniversityManagementSystemMicroservice.git
   cd UniversityManagementSystemMicroservice
   ```

2. **Restore Dependencies & Build:**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run Services Simultaneously:**
   You can launch the solution in Visual Studio with Multiple Startup Projects configured, or use the .NET CLI:
   ```bash
   dotnet run --project "API Gateway/API Gateway.csproj"
   dotnet run --project "AcademicService/AcademicService.csproj"
   dotnet run --project "AttendanceService/AttendanceService.csproj"
   dotnet run --project "ExamService/ExamService.csproj"
   dotnet run --project "EmailService/EmailService.csproj"
   ```

---

## 👨‍💻 Author

**Omar Alfarouk**  
*Full-Stack .NET & Software Engineer*  

- 🌐 **GitHub:** [@OmarAlfar0uk](https://github.com/OmarAlfar0uk)
- 💼 **LinkedIn:** [omar-alfarouk](https://www.linkedin.com/in/omar-alfarouk-252471251/)
- 📧 **Email:** [omaralfarouk646@gmail.com](mailto:omaralfarouk646@gmail.com)

---

<div align="center">
  <sub>Built with ❤️ by Omar Alfarouk. Licensed under the <a href="LICENSE">MIT License</a>.</sub>
</div>
