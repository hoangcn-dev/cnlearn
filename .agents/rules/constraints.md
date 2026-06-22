---
trigger: always_on
---

# 1. Project Context & Stack
- **Exception System**: Custom exceptions inheriting from `BaseCustomException` in `HoangCN.Core.Common.Exceptions` (such as `BadRequestException`, `NotFoundException`, `UnauthorizedException`) must be used.
- **Configuration Structure**: Centralized configuration management combining static configuration (`appsettings.json`) and environment variables, loaded via `IOptions<T>` to ensure security and easy deployment across multiple environments.

# 2. Architecture Guidelines
- **Global Middleware/Filter**: A global Exception Middleware/Filter must be used to catch and convert Custom Exceptions into a consistent JSON error format returned to clients.
- **No Manual Try-Catch in Controllers**: Manual `try-catch` blocks are prohibited in the Controller layer. All exceptions (both custom and system exceptions) must propagate to the global Exception Filter/Middleware, unless a specific business case requires overriding the error code (approval is required).
- **Data Flow Separation**: Controllers must only receive requests and forward them to Services (BL). Database operations or complex business calculations are strictly prohibited inside Controllers.

# 3. Coding Conventions & Best Practices
- **Logic Coding Rules**:
  - **Minimalist Logic**: Write processing logic as simple as possible, avoiding deeply nested complex conditional branches.
  - **Happy Path**: Focus only on checking main flows (happy paths & mandatory validation branches).
  - **No Fallback Configurations**: Do not write fallback conditions/configurations in the code.
  - **Ignore Impossible Conditions**: Omit check conditions that can never happen to keep the code clean and optimize performance.
  - **Throw Exceptions Directly**: For business errors, invalid authentication, or branch deviations, **throw exceptions directly** instead of returning null or empty fallback values.
- **Code Reuse**:
  - **Prioritize Reuse**: Maximize the reuse of existing code when business operations are simple and similar.
  - **Custom & Common Functions**: When writing complex custom code or new common helper/utility functions, approval is required.
- **No Hard-coding**:
  - **Strict Prohibition**: Hard-coding system configuration values (such as API keys, DB connection strings, token expiration, sender emails...) is strictly prohibited.
  - **Dynamic Configuration Loading**: All configurations must be loaded via `appsettings.json` combined with `IOptions<T>` or the environment utility `EnvUtil`.

# 4. Agent Behavior & Workflows
- **Clarify Business Requirements**:
  - Do not write or edit code when the business requirements are unclear.
  - Stop and ask the user to clarify requirements; guessing is prohibited.
- **Plan Before Coding (Planning)**:
  - You must create or update the Implementation Plan (`implementation_plan.md`) before implementing a new feature or making complex structural changes.
  - Only make source code changes after the user has reviewed and approved the plan.
- **Automatic Version Control History (Git Commit)**:
  - Automatically stage and commit changes for each successfully completed feature (after the user approves/verifies or when preparing for the next task).
  - Clearly display the commit command and message to the user for review.

# 5. Testing Standards
- **Exception Scenario Testing**: When writing unit tests for error paths or exception throwing, `Assert.ThrowsAsync<TException>` must be used to verify that the system throws the correct exception type.
- **Test Method Naming**: Follow the naming convention `MethodName_Should_ExpectedResult_When_Condition` to improve clarity of test cases.
