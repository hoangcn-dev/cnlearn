# SKILL: BUILD_AND_RUN_IMPLEMENT_PLAN
- **Description**: Skill for building source code implementation plans.
- **Trigger**: After the user approves the preliminary description and an implementation plan is required.

## CONTEXT & CONSTRAINTS
- Strictly adhere to the proposed implementation plan.
- Only build implementation plans for relevant files within the approved scope.
- Do not make unauthorized modifications that are not present in the business description.

## WORKFLOW
1. Reread the architecture, constraints, and relevant code files to grasp the context and requirements.
2. Determine whether it is necessary to add Entities; prioritize adding fields to existing entities, and only add new entities if it represents a distinct business domain.
3. Based on the business features in the description, build the corresponding service including the Service Interface + Service Implementation.
4. Consider whether to add new methods or override existing methods from `BaseBL`.
5. Build DTOs and Requests matching the new methods in the Service Interface.
6. Create a new controller or add to an existing controller if they fall within the same business scope.
7. Run Unit Tests following the [build-and-run-unit-test] skill.
8. Request the user to check changes and confirm. Revise based on feedback.
9. Continue with Frontend implementation once approved.
10. Identify APIs that need to be added/modified.
11. Add/modify the corresponding frontend models. Ensure proper mapping with Backend APIs.
12. Add/modify Views, creating new Components if necessary.
13. Request the user to check changes and confirm. Revise based on feedback.
14. Perform commit confirmation following the [commit] skill.

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Always explain carefully about the changes that have been made.