# SKILL: REFACTOR
- **Description**: Skill for improving and optimizing source code structure without changing its business behavior.
- **Trigger**: When code contains duplication, is over-complicated, too long, hard to maintain, or violates design patterns.

## CONTEXT & CONSTRAINTS
- Strictly prohibited from changing the external behavior of the system (business outcomes must remain 100% identical).
- When writing new shared helper/utility functions or making major structural changes, approval is required before proceeding.
- Code after refactoring must strictly comply with naming rules, private fields, async/await guidelines in `coding.md` and project constraints `constraints-*.md`.

## WORKFLOW
1. **Understand Current State**:
   - Read and fully understand the business logic of the code block targeted for refactoring.
2. **Ensure Test Coverage**:
   - Check existing Unit Tests. If none exist or coverage is low, write test cases first to act as a behavioral safeguard.
3. **Plan Refactoring**:
   - Build a detailed plan (following the [build-and-run-implement-plan] skill):
     - Identify duplicate code blocks to consolidate.
     - Simplify verbose branching structures (applying Return Early and happy paths).
4. **Micro-refactoring**:
   - Make small, incremental changes, running Unit Tests continuously after each small change to detect issues immediately.
5. **Verify Correctness**:
   - Run all relevant Unit Tests following the [build-and-run-unit-test] skill.
6. **Commit Changes**:
   - Perform a Git commit following the [commit] skill.

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Explain the benefits of refactoring (e.g., performance gains, reduced duplication, improved readability) and the impacted code areas.
