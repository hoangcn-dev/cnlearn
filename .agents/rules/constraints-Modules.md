---
trigger: always_on
---

# Constraints of Business Modules (HoangCN.MainSystem & HoangCN.LearnMS & HoangCN.*)

## General Module Project Constraints
- Must reference `Core.BL`, `Core.DL`, and `Core.Common`.
- API Controllers are strictly prohibited from calling the DL layer (`Core.DL`) directly. All database communication must go through the BL layer (Services).
- Modules are strictly prohibited from referencing each other under any circumstances.
- All modules must comply with the component architecture (described in 4.*).

## Detailed Constraints by Folder

### 4.1 Controllers
- Only call Services (BL layer) for processing; must not contain business logic calculations or direct database operations.
- Must inherit from `CRUDController<TEntity>` of the base BL layer if providing basic CRUD operations. Otherwise, must inherit from `ControllerBase`.
- Implement centralized authorization by overriding the `ConfigurePolicies(AuthActionPolicyBuilder builder)` method. For example: use `builder.Protect(nameof(ActionName), nameof(RoleNames.RoleName))` to configure specific access permissions for inherited endpoints.
- Custom API methods must be annotated with specific routing attributes (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`) along with detailed endpoint paths.
- Responses must be wrapped in `ApiResponseDto.Success(...)` or `ApiResponseDto.Error(...)` (if not using automatic exception filters) to ensure a consistent returned data structure for clients.
- Do not use try/catch blocks in controllers; leave exception handling entirely to the exception filter.

### 4.2 Interfaces
- Comment thoroughly on the functionality of each method.
- Methods with no return values: void/Task.
- Methods with return values: Task<TDto> where TDto is a DTO defined in 4.4.
- Method parameters: If wrapping data with more than 1 field, it must be a Request defined in 4.5. If the parameter has only 1 field, a primitive data type can be used.

### 4.3 Services
- Must inherit from `BaseBL<TEntity>` (if working with Entities) and implement the corresponding interface (e.g., `IQuestionService`).
- All helper/internal methods not declared in the inherited interface must be placed inside the `#Internal` region.
- All overridden methods must be placed inside the `#Override` region.
- Must not contain hardcoded SQL strings; call the dynamic builder `BuildSQLUtil` in Utils.
- Comment concisely on processing methods.
- Code comments within methods must be direct and brief, explaining what the block of code does. Avoid long-windated comments.
- Prioritize code reuse and minimize duplicate code.
- Each method must perform only one single task.
- Internal method names must be clear and represent their function.

### 4.4 DTOs
- Must not contain complex business logic.
- Class names must end with the suffix `Dto`.
- Field names must correspond as closely as possible to field names in the matching Entity.
- Fields retrieved from foreign tables must have the attribute `[ForeignTable(EntityType = typeof(EntityName), ColumnName = "ColumnName")]` (ColumnName is only needed if the database column name differs from the field name).
- Comment briefly in a single line on the Request class and all fields.

### 4.5 Requests
- Must not contain processing logic.
- Class names must end with the suffix `Request`.
- Fields requiring non-null/non-empty values must have the attribute `[Required(ErrorMessage = "Trường {0} không được để trống")]`.
- String fields requiring specific lengths must have the attribute `[StringLength(MinLength = 1, MaxLength = int, ErrorMessage = "Trường {0} không được vượt quá {1}-{2} ký tự")]` ({1} defaults to 1 if MinLength is omitted).
- Email/Phone fields must have the attribute `[EmailAddress(ErrorMessage = "Trường {0} không đúng định dạng")]` (or `[Phone]`).
- Comment briefly in a single line on the Request class and all fields.
- All fields must have the attribute `[DisplayName("Vietnamese Field Name")]`.

### 4.6 Entities
- Must inherit from `BaseEntity`.
- Entity names must not be plural (e.g., `Question`, not `Questions`).
- Intermediate/mapping entities representing many-to-many tables must have the suffix `Mapping`.
- Tables managing a specific object must share a common prefix `EntityName` (e.g., `Question`, `QuestionAnswer`, `QuestionAnswerDetail`).
- The following fields must be prefixed with the Entity name:
  - Name => e.g., `QuestionName`
  - Code => e.g., `QuestionCode`
  - Id => e.g., `QuestionId`
  - Slug => e.g., `QuestionSlug`
- The primary key field `EntityId` must be annotated with the `[Key]` attribute.
- Comment briefly in a single line on the Entity class and all fields.
- All fields must have the attribute `[DisplayName("Vietnamese Field Name")]`.
- Foreign key fields must have the attributes `[ForeignKey(nameof(ParentEntity))]` and `[CheckExist(MustExist = true, TargetEntity = typeof(ParentEntity), ErrorMessage = "Trường {0} không tồn tại trong hệ thống.")]`.
- Minimize using navigation object fields; if present, they must be marked with `[NotMapped]`.
- Other attributes are similar to Requests (4.5).
- Configure `[Index]` for fields frequently searched or with natural indexes.
- Configure `[SearchConfig]` for custom default sorting configurations.

### 4.7 DB
- Contains DB configuration classes, DB connection initializers, or inherits from `DynamicDbContext` of the DL layer.
- Implement `IDesignTimeDbContextFactory<DynamicDbContext>` (e.g., `DynamicDbContextFactory`) to provide DbContext configurations during development (design-time), serving the creation of database migration updates independently for each module.
- Connection strings used in design-time factories can have local default configurations, but runtime connections must be loaded from environment variables via `EnvUtil`.

### 4.8 Enums
- Must reside in the namespace `HoangCN.<ModuleName>.Enums`.
- Must have thorough XML documentation comments (`/// <summary>`) on both the Enum class and all members to support Swagger/API document generation and IDE code completion.
- Enum values must be explicitly assigned integer values (`int`) starting from `0` or according to business conventions.

### 4.9 Utils
- Contains the `DI.cs` file defining the extension method `Add<ModuleName>(this IServiceCollection services, IConfiguration configuration)` to register all Services, HostedServices, and configurations of the module into the DI Container.
- Contains the `EnvKeys.cs` file declaring constants for the module's environment variable configuration keys (e.g., connection strings, secret keys).
- Contains query utility classes such as `*SqlUtil.cs` (e.g., `QuestionSqlUtil.cs`). All Raw SQL/Dapper queries of the module must be centralized in static methods in these utility classes using `DynamicParameters` to parameterized queries and prevent SQL Injection. Hardcoded SQL strings directly inside Service classes are strictly prohibited.
