---
trigger: always_on
---

# 1. Project Context & Stack
- **C# / .NET 8**: Uses standard Microsoft coding conventions combined with project-specific naming conventions for DTOs and Requests.
- **Vue 3 / TypeScript**: Strictly adheres to TypeScript and Vue 3 Style Guide (Composition API).

# 2. Architecture Guidelines
- DTO (Data Transfer Object) and Request/Response payload must not contain business logic.
- Entities must inherit from `BaseEntity` (which contains Audit fields: `CreatedBy`, `CreatedDate`, `ModifiedBy`, `ModifiedDate`, `IsDeleted`).

# 3. Coding Conventions & Best Practices
- **C# (Backend)**:
  - **Naming**:
    - Class, Interface, Method, Property, Enum: `PascalCase` (e.g., `QuestionService`, `IQuestionService`, `GetById`, `QuestionId`).
    - Private fields: `_camelCase` (e.g., `_baseReadDL`, `_logger`).
    - Local variables & Parameters: `camelCase` (e.g., `entities`, `currentUserId`).
    - Interfaces must start with the prefix `I` (e.g., `IBaseBL`).
  - **Class Conventions**:
    - DTO: Ends with the suffix `Dto` (e.g., `BankQuestionDto`).
    - Request Payload: Ends with the suffix `Request` (e.g., `SaveQuestionsRequest`).
  - **Async/Await**:
    - All I/O operations (Database, Network, File) must be written asynchronously using `async` and return `Task` or `Task<T>`.
    - Do not block threads using `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.
- **TypeScript & Vue 3 (Frontend)**:
  - Component filenames: `PascalCase` (e.g., `QuestionBankView.vue`).
  - Variables & Functions: `camelCase` (e.g., `getQuestions`, `isLoading`).
  - TypeScript: Explicitly define interfaces for DTOs (e.g., `src/models/questions.ts`), avoid using `any`.

# 4. Agent Behavior & Constraints
- Logic code must be as simple as possible, avoiding nesting too many complex conditional branches.
- When generating new code, follow the style of similar existing files in the system (e.g., inherit from `BaseBL` and call existing utility methods).

# 5. Testing Standards
- Test method naming convention: `MethodName_Should_ExpectedResult_When_Condition` (e.g., `SignIn_ShouldThrowUnauthorizedException_WhenUserIsLocked`).
