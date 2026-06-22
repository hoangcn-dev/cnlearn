# SKILL: FIX_BUG
- **Description**: Skill for diagnosing, pinpointing root causes, and resolving bugs.
- **Trigger**: When the user reports a bug, a crash, or a unit test fails and requires fixing.

## CONTEXT & CONSTRAINTS
- Only fix the exact cause of the bug; write processing logic as simple as possible and avoid side-effects on unrelated business domains.
- Do not write try-catch blocks manually in Controllers to swallow errors or return empty fallbacks (you must throw appropriate exceptions to let the Exception Middleware handle them).
- Do not make arbitrary guesses about business logic when fixing bugs. If the cause of the bug points to unclear business requirements, you must ask the user to clarify.

## WORKFLOW
1. **Reproduce**:
   - Use information from the bug description, system logs, or write a minimalist Unit Test to reproduce the bug to identify the exact faulty behavior.
2. **Pinpoint**:
   - Analyze stack traces, logs, or debug the code to find the exact line of code/logic causing the issue.
   - Cross-reference with architecture guidelines and coding rules to see if any constraints were violated.
3. **Plan**:
   - Propose a minimalist, safe, and low-risk fix.
   - If the changes are complex or span multiple files, write an implementation plan following the [build-and-run-implement-plan] skill for user approval.
4. **Apply Fix**:
   - Implement source code modifications according to the approved plan.
5. **Verify**:
   - Rerun existing Unit Tests and the reproducing test case following the [build-and-run-unit-test] skill to ensure they pass and do not introduce regressions.
6. **Commit Changes**:
   - Perform a Git commit following the [commit] skill.

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Clearly explain the root cause and the fix applied.
