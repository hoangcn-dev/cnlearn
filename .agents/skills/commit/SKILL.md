# SKILL: COMMIT
- **Description**: Skill for staging and creating Git commits.
- **Trigger**: After a feature or change is successfully implemented (verified and approved by the user) and ready to transition to the next set of tasks.

## CONTEXT & CONSTRAINTS
- Only commit changes directly related to the current task; avoid committing temporary, generated, or unrelated files.
- Commit messages must be written in English and follow the **Conventional Commits** standard:
  - `feat`: New feature.
  - `fix`: Bug fix.
  - `docs`: Documentation updates.
  - `style`: Code style/formatting changes (whitespace, formatting, missing semi-colons, etc.).
  - `refactor`: Refactoring code without changing business behavior.
  - `test`: Adding or modifying tests.
  - `chore`: Build configuration, dependency updates, tools, etc.
- Must display the commit command and commit message to the user for visibility.

## WORKFLOW
1. Check changed files status by running:
   ```bash
   git status
   ```
2. Add target files directly related to the current task to the Staging Area:
   ```bash
   git add <file_path_1> <file_path_2> ...
   ```
3. Compose a concise and meaningful commit message using the format: `<type>: <description>` (e.g., `feat: add user authentication with JWT` or `docs: complete architecture guidelines`).
4. Execute the commit:
   ```bash
   git commit -m "<commit_message>"
   ```
5. Display a success notification along with the commit summary to the user.
6. **Fallback for Terminal Restriction**: If permission to execute terminal commands on the user's local machine is denied, you must clearly output the exact `git add` and `git commit` commands in the final response so the user can easily run them manually.

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Clearly present the git commands to be executed for transparency.
