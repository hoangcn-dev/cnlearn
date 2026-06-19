# SKILL: WRITE_DOCUMENTATION
- **Description**: Skill for creating and updating project documentation.
- **Trigger**: When the user requests to write, update, or generate system documentation, code templates, or feature specifications.

## CONTEXT & CONSTRAINTS
- All documentation files must be stored inside the `docs/` directory at the project root. If the `docs/` directory does not exist, create it.
- Every documentation file must be in Markdown (`.md`) format.
- Every file name must strictly follow the prefix convention below based on the documentation type:
  - `feat-*.md`: For major business feature specifications or requirements (e.g., `feat-exam-grading.md`).
  - `architecture-*.md`: For system architecture, folder layouts, or service relationship descriptions (e.g., `architecture-dl-layer.md`).
  - `template-*.md`: For code skeletons, boilerplate templates, or implementation examples (e.g., `template-crud-service.md`).

## WORKFLOW
1. Clarify the documentation requirements with the user if they are ambiguous. Do not guess.
2. Determine the correct file name prefix based on the documentation type:
   - Use `feat-` prefix for functional business specs.
   - Use `architecture-` prefix for technical designs/architectures.
   - Use `template-` prefix for code patterns or samples.
3. Check if the target file already exists in `docs/`:
   - If it exists, read the file to understand the current documentation style.
   - If it does not exist, prepare to create it.
4. Author the document content using clean, structured Markdown, utilizing headers, bullet points, code blocks, or diagrams where appropriate.
5. Save the file to the path `docs/<prefix>-<name>.md`.
6. Present the drafted documentation to the user and request their feedback/review.
7. Refine the documentation based on user feedback.
8. Once finalized and approved by the user, follow the [commit] skill to commit the changes.

## USER COMMUNICATION RULE
- Always use Vietnamese to communicate with the user.
- Highlight the created/updated documentation file path using clickable markdown links.
