---
trigger: always_on
---

# 1. Project Context & Stack
- **Backend Stack**:
  - **Framework**: .NET Core 8 Web API.
  - **ORM / Database Access**: EF Core (for command/write) & Dapper (for query/read optimization).
  - **Caching & Session**: Redis.
  - **Security / Authentication**: JWT (JSON Web Token), Cookie-based auth.
  - **Testing**: xUnit, Moq, Custom Fakes.
- **Frontend Stack**:
  - **Framework**: Vue 3 (Composition API), Vite, TypeScript.
  - **State Management**: Pinia.
  - **Router**: Vue Router.
  - **Styling**: Vanilla CSS (highly optimized and flexible).

# 2. Project Goals & Target
- **What the project is doing**:
  - The project is a multi-system/multi-module solution including:
    - **MainSystem**: Authentication gateway, user authorization management, background services (email, redis caching).
    - **LearnMS**: Learning Management System, managing question banks, exams, and student exam sessions.
- **Goals of the project**:
  - Build a Clean Architecture with high performance by separating read (Dapper) and write (EF Core) flows.
  - Optimize code reuse across backend modules using the common core (`HoangCN.Core`).
  - Provide a smooth and intuitive exam and learning management experience on the Vue 3 Frontend.
