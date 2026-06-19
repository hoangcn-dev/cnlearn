---
trigger: always_on
---

# 1. Project Context & Stack
- **Testing Framework**: xUnit.
- **Mocking Library**: Moq (for mocking dependent services such as `IHttpContextAccessor`, `IEmailService`, `IRedisService`).
- **Fakes**: `FakeReadDL` and `FakeWriteDL` must be used to replace the database in the Service layer Unit Tests.

# 2. Architecture Guidelines
- Do not make connections to real databases or real file systems in unit tests.
- All database interactions must be isolated through Fake/Mock layers.

# 3. Coding Conventions & Best Practices
- **Test Method Naming**: `MethodName_Should_ExpectedResult_When_Condition`
  - Example: `BeforeUpdate_ShouldRetainOldPasswordAndSalt_WhenPasswordInPayloadIsEmpty`.
- **Test Structure (3A Pattern)**:
  - `Arrange`: Prepare mock data, mock dependent services, and populate the Fake DL.
  - `Act`: Call the target method to test directly from the service.
  - `Assert`: Verify outputs (using `Assert.Equal`, `Assert.True`, `Assert.Contains...`) or check if exceptions are thrown correctly (`Assert.ThrowsAsync<TException>`).
- **Use `#region`**: Group test cases by target method name to maintain clean organization.

# 4. Agent Behavior & Constraints
- When adding a new feature or business method, you must write corresponding test scenarios (including happy paths and exception scenarios).
- Ensure all tests are independent and do not affect each other's state.

# 5. Testing Standards
- Avoid over-mocking the DL Interfaces. Prioritize using `FakeReadDL` and `FakeWriteDL` by adding mock data directly to the mock list (e.g., `_fakeReadDL.QueryResults.Add(...)`) to keep the code concise and clean.
