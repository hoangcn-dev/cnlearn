# SKILL: BUILD_AND_RUN_UNIT_TEST
- **Description**: Skill for building and running Unit Tests.
- **Trigger**: After Backend/Service source code changes or additions have been made and verification is required.

## CONTEXT & CONSTRAINTS
- Only write Unit Tests for the business layer (Service); making connections to real databases, real Redis, or real File Systems is prohibited.
- All database interactions and environments must be isolated using Mock layers (using `Moq` library) and Fake layers (`FakeReadDL`, `FakeWriteDL`).
- Avoid over-mocking DL Interfaces. Prioritize using `FakeReadDL` and `FakeWriteDL` by adding mock data directly to the mock list (e.g., `_fakeReadDL.QueryResults.Add(...)`).
- Test structures must strictly follow the **3A Pattern** (Arrange, Act, Assert).
- Test method naming convention: `MethodName_Should_ExpectedResult_When_Condition` (e.g., `BeforeUpdate_ShouldRetainOldPasswordAndSalt_WhenPasswordInPayloadIsEmpty`).
- Must group test cases of the same business method inside the same `#region` named after the target business method.
- When testing error paths or exception scenarios, `Assert.ThrowsAsync<TException>` must be used to verify that the system throws the correct exception type.

## WORKFLOW
1. Identify the unit test project corresponding to the module being tested (e.g., `HoangCN.MainSystem.Tests`).
2. Open the matching test file or create a new one if it does not exist (e.g., `UserServiceTests.cs` matching `UserService`).
3. Prepare mock data and configure mock/fake objects (the **Arrange** phase):
   - Add query mock data to `_fakeReadDL.QueryResults` or `_fakeWriteDL.Entities`.
   - Configure mock methods for dependencies using `Mock<IService>.Setup(...)` if any.
4. Call the target business method to test (the **Act** phase).
5. Verify the returned results or thrown exceptions (the **Assert** phase):
   - Use `Assert.Equal`, `Assert.True`, `Assert.NotNull` to check returned values or state changes in Fake DL.
   - Or use `Assert.ThrowsAsync<TException>` for verifying expected exception triggers.
6. Run tests by executing the command:
   ```bash
   dotnet test
   ```
7. If any test case fails, debug, revise implementation or tests, and rerun until all tests pass (reach green status).

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Clearly report the list of test cases executed and their outcomes (Passed/Failed).
