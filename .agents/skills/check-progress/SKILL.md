# SKILL: CHECK_PROGRESS
- **Description**: Skill for checking and verifying the task progress of the agent.
- **Trigger**: When the user requests to verify progress, or at the start of a task to review current status, or before finalizing work to ensure all tasks are completed.

## CONTEXT & CONSTRAINTS
- Use `task.md` as the primary source of truth for the list of planned tasks.
- Cross-reference the checklist with actual workspace modifications using `git status` and `git diff`.
- Ensure there are no compilation errors and unit tests are passing before reporting a task as complete.

## WORKFLOW
1. Locate and read the `task.md` file in the workspace or the brain/artifacts directory to review the checklist.
2. If `implementation_plan.md` exists, read it to understand the technical details and scope of the changes.
3. Run `git status` to see the current modified, deleted, and untracked files.
4. Compare the changed files against the checked/completed items in `task.md`:
   - If a task is marked as completed (`[x]`) but no corresponding files were modified or created, verify if it was really done.
   - If files are modified but the task is not checked, update `task.md` accordingly.
5. Compile/build the project to verify that the changes did not break the build.
6. (Optional but recommended) Run relevant unit tests to verify correctness of the implemented logic.
7. Prepare a clear, structured progress report in Vietnamese for the user, listing:
   - **Completed tasks** (Công việc đã hoàn thành).
   - **Tasks in progress** (Công việc đang thực hiện).
   - **Remaining tasks** (Công việc chưa thực hiện).
   - **Issues or roadblocks** (Vấn đề phát sinh, nếu có).

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Keep the progress report clear, bulleted, and professional.
