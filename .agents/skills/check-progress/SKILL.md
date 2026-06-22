# SKILL: CHECK_PROGRESS
- **Description**: Skill for checking and verifying the task progress of the agent.
- **Trigger**: When the user requests to verify progress, or at the start of a task to review current status, or before finalizing work to ensure all tasks are completed.

## CONTEXT & CONSTRAINTS
- Use `task.md` as the primary source of truth for the list of planned tasks.
- Cross-reference the checklist with actual workspace modifications using `git status` and `git diff`.
- Identify and specify the final list of modified source code files, including their exact locations (relative paths or clickable markdown links) and a summary/diff of the changed content.
- Ensure there are no compilation errors and unit tests are passing before reporting a task as complete.

## WORKFLOW
1. Locate and read the `task.md` file in the workspace or the brain/artifacts directory to review the checklist.
2. If `implementation_plan.md` exists, read it to understand the technical details and scope of the changes.
3. Run `git status` and `git diff --name-only` to identify all modified source code files in the workspace.
4. For each modified source code file, run `git diff <file_path>` or examine the differences to identify the exact locations and content changes made.
5. Compare the changed files against the checked/completed items in `task.md`:
   - If a task is marked as completed (`[x]`) but no corresponding files were modified or created, verify if it was really done.
   - If files are modified but the task is not checked, update `task.md` accordingly.
6. Compile/build the project to verify that the changes did not break the build.
7. (Optional but recommended) Run relevant unit tests to verify correctness of the implemented logic.
8. Prepare a clear, structured progress report in Vietnamese for the user, listing:
   - **Completed tasks** (Công việc đã hoàn thành).
   - **Tasks in progress** (Công việc đang thực hiện).
   - **Remaining tasks** (Công việc chưa thực hiện).
   - **Modified files and changes** (Danh sách các file mã nguồn cuối cùng được sửa đổi, bao gồm vị trí và tóm tắt nội dung thay đổi).
   - **Issues or roadblocks** (Vấn đề phát sinh, nếu có).

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Keep the progress report clear, bulleted, and professional.
