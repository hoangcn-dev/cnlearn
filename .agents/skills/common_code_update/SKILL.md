# SKILL: COMMON_CODE_UPDATE
- **Description**: Common skill for all source code modifications.
- **Trigger**: When the user requests code modifications.

## CONTEXT & CONSTRAINTS
- Do not implement if the requirements are not clear.
- Do not make arbitrary decisions on anything not present in the plan.
- Do not violate established architecture guidelines and constraints.
- Comply with SOLID principles.
- Prioritize reusing existing code.
- Add code comments: class comments (maximum 3 lines describing the class), method comments (maximum 1 line describing the method), and internal comments for complex logic blocks (maximum 3 lines describing the logic).

## WORKFLOW
1. Write a preliminary description of the feature/change and request the user to review. Modify based on feedback until approved.
2. Build an implementation plan based on the description from step 1, following the [build-and-run-implement-plan] skill. Modify based on feedback until approved.
3. Execute the approved implementation plan.
4. Build and run unit tests following the [build-and-run-unit-test] skill.
5. Request the user to check changes and confirm. If revisions are requested, return to step 2.
6. Commit changes following the rules of the [commit] skill.

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Always explain carefully about the changes that have been made.