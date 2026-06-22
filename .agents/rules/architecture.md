---
trigger: always_on
---

# 1. Project Context & Stack
- The system is organized as a multi-project Solution (.SLN), combining **C# ASP.NET Core** on the Backend and **Vue 3 (Vite + TypeScript)** on the Frontend.

# 2. Architecture Guidelines

## A. Layer Decomposition by Project in the Solution

### 1. HoangCN.Core.Common (Common Core Layer)
*See detailed technical constraints for this layer at [constraints-HoangCN.Core.Common.md](./constraints-HoangCN.Core.Common.md).*

- **Role**: Provides base entities, custom exceptions, enums, and common utility libraries for the entire Solution.

#### 1.1 Attributes
- **Role**: Defines metadata for Entity properties to support Reflection analysis.

#### 1.2 Base
- **Role**: Defines the base entity (`BaseEntity`) containing common Audit properties (`CreatedBy`, `CreatedDate`, `ModifiedBy`, `ModifiedDate`, `IsDeleted`).

#### 1.3 Enums
- **Role**: Defines sets of shared constants (e.g., status, question types, access levels).

#### 1.4 Exceptions
- **Role**: Declares custom business exceptions (`BadRequestException`, `NotFoundException`, `UnauthorizedException`).

#### 1.5 Model
- **Role**: Defines DTOs and Requests common to the entire project (such as `GetRequest`, `ResultDto`, `Filter`).

#### 1.6 Utils
- **Role**: Provides stateless utility functions such as `ClaimUtil` (retrieving user claims), `SlugUtil` (generating Vietnamese slugs), and `PasswordUtil` (hashing passwords).

---

### 2. HoangCN.Core.DL (Data Access Layer)
*See detailed technical constraints for this layer at [constraints-HoangCN.Core.DL.md](./constraints-HoangCN.Core.DL.md).*

- **Role**: Manages database connections, executes SQL queries, and manages transactions.

#### 2.1 Interfaces
- **Role**: Defines abstract data access behaviors (`IBaseReadDL`, `IBaseWriteDL`).

#### 2.2 Implementation
- **Role**: Implements the data access interfaces. `BaseReadDL` uses Dapper to optimize read speeds (Query). `BaseWriteDL` uses EF Core to execute write operations (Command) and manage Transactions.

#### 2.3 DynamicDbContext
- **Role**: The DbContext configuration class for EF Core, which dynamically maps Entities to their corresponding database tables.

---

### 3. HoangCN.Core.BL (Business Logic Layer)
*See detailed technical constraints for this layer at [constraints-HoangCN.Core.BL.md](./constraints-HoangCN.Core.BL.md).*

- **Role**: Provides basic business logic, coordinates transactions, performs pre/post data processing hooks, and configures Metadata.

#### 3.1 Base
- **Role**: Implements the base class `BaseBL<TEntity>`. Automatically validates data (`ValidateUtil`), assigns Audit information in hooks (`BeforeInsert`, `BeforeUpdate`), and performs basic CRUD.

#### 3.2 Metadata
- **Role**: Caches Entity structural information (`EntityMetadataCache`) through Reflection (such as primary key column name and attributes) to optimize performance.

#### 3.3 Utils
- **Role**: Builds dynamic SQL (`BuildSQLUtil`) and translates Lambda Expressions to filter structures (`ExpressionToFilterTranslator`).

---

### 4. Business Module Projects (HoangCN.LearnMS & HoangCN.MainSystem & HoangCN.*)
*See detailed technical constraints for module projects at [constraints-Modules.md](./constraints-Modules.md).*

- **Role**: Detailed business sub-systems of the application (e.g., Learning Management System, Admin Portal).
- **General Description of Modules**:
  - HoangCN.MainSystem: Builds shared and core services for the system, including:
    - User management and authentication
    - Mail sending service
    - File management service
    - Permission management service
  - HoangCN.LearnMS: Builds services for the learning management system, including:
    - Question Bank + Exam management
    - Online exam organization
    - Courses

#### 4.1 Controllers
- **Role**: Entry point for receiving client requests, handling routing, and defining API endpoints to process system resources.

#### 4.2 Interfaces
- **Role**: Defines interfaces for Services.

#### 4.3 Services
- **Role**: Implements detailed business logic for each module (e.g., `QuestionService` handles grading logic, saving answers, Excel import).

#### 4.4 DTOs
- **Role**: Contains returned data structures.

#### 4.5 Requests
- **Role**: Contains data structures sent via APIs. (Some APIs will directly use the Entity defined in 4.6).

#### 4.6 Entities
- **Role**: Declares entities representing the module's own database tables (such as `Question`, `Exam`, `QuestionAnswer`).

#### 4.7 DB
- **Role**: Manages the database of each specific module, configures database tables, sets up relationships, and manages independent migration versions.

#### 4.8 Enums
- **Role**: Declares specific business constant enums for the module (such as exam attempt status `ExamSessionStatus`, question type `QuestionType`).

#### 4.9 Utils
- **Role**: Provides Dependency Injection configuration utilities, environment variable management, and SQL query generation utilities for the DL layer (Dapper).

---

### 5. Webs (CNLearnMS & CNAdmin Frontend Clients)
*See detailed technical constraints for frontend clients at [constraints-Webs.md](./constraints-Webs.md).*

- **Role**: SPA user interface developed with Vue 3 + TS.

#### 5.1 api
- **Role**: Defines HTTP RESTful API call functions (using Axios) to communicate with Backend endpoints.

#### 5.2 assets
- **Role**: Contains static assets of the application such as CSS/Sass, images, icons, and fonts.

#### 5.3 components
- **Role**: Contains shared and reusable Vue UI Components for the entire project.

#### 5.4 models / model
- **Role**: Defines data structures as TypeScript interfaces or types to enforce input/output data typing.

#### 5.5 router
- **Role**: Configures screen navigation routes and navigation guards to check access permissions.

#### 5.6 stores
- **Role**: Stores and manages centralized state for the entire Vue 3 application using Pinia.

#### 5.7 utils
- **Role**: Provides stateless common utility functions (Stateless helpers).

#### 5.8 views
- **Role**: Contains main screen files (Page/View components) corresponding to each router path.