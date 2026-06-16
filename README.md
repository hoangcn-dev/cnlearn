# HoangCN Online Learning & Quiz System

A modern online learning and quiz ecosystem built using a multi-project/microservices architecture.

---

## 📂 Project Structure

The workspace consists of the following key components:

### 1. Backend (.NET 8 Web API)
*   **HoangCN.MainSystem**: Core management system and user authentication.
*   **HoangCN.LearnMS**: Learning management module storing question banks, exams, and hosting online real-time exam sessions.
*   **HoangCN.Core.BL (Business Logic)**: Contains the core business logic of the system.
*   **HoangCN.Core.DL (Data Layer)**: Handles database communication using Entity Framework Core.
*   **HoangCN.Core.Common**: Houses base classes, utility classes, and common middlewares (such as the exception handling middleware `CatchExceptionMiddleware`).
*   **Module.MediaDownloader**: A sub-system to manage and download multimedia assets.

### 2. Frontend (Vue 3 / Vite)
All web applications are located within the `Webs/` directory:
*   **Webs/CNLearnMS**: The student learning and exam-taking portal, featuring rich LaTeX/KaTeX mathematical formula rendering.
*   **Webs/CNAdmin**: The administration panel for teachers and administrators.
*   **Webs/CNNovels**: A platform for reading novels and reference materials.

---

## 🛠️ Technology Stack

*   **Backend**: C#, .NET 8 SDK, Entity Framework Core, Pomelo.EntityFrameworkCore.MySql, ASP.NET Core Web API.
*   **Frontend**: Vue 3 (Composition API), TypeScript, Vite, Ant Design Vue, Pinia, Axios, KaTeX/Markdown-it.
*   **Database**: MySQL / MariaDB.

---

## 🚀 How to Run the Project

### Prerequisites
*   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   [Node.js](https://nodejs.org/) (v20 or newer recommended)
*   A running instance of MySQL/MariaDB.

### 1. Launch All Services Simultaneously (Recommended)
A `dev.sh` shell script is provided to boot all four main applications at once in watch/hot-reload mode (run this using Git Bash or any bash-compatible shell on Windows):

```bash
./dev.sh
```

*Press `Ctrl+C` in the terminal to stop all running background services simultaneously.*

### 2. Launch Services Manually

#### Run Backend APIs:
Open separate terminals for each service:
```bash
# Start MainSystem
cd HoangCN.MainSystem
dotnet watch run

# Start LearnMS
cd HoangCN.LearnMS
dotnet watch run
```

#### Run Frontend Apps:
Open separate terminals for each web client:
```bash
# Start Student Portal (CNLearnMS)
cd Webs/CNLearnMS
npm run dev

# Start Administration Panel (CNAdmin)
cd Webs/CNAdmin
npm run dev
```

---

## ⚙️ Configuration (Environment / AppSettings)
*   **Backend**: Configure your MySQL connection strings in `appsettings.json` or `appsettings.Development.json` within each API project.
*   **Frontend**: Configure the backend API endpoints in `.env.development` within each web project (e.g., `VITE_API_BASE_URL=http://localhost:5006`).