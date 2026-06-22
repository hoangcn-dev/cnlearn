---
trigger: always_on
---

# Constraints of Project Webs/CNLearnMS & Webs/CNAdmin (Frontend Client)

## General Project Constraints
- Completely independent of the Backend; must only interact with data via HTTP RESTful APIs exposed by the Backend endpoints.

## Detailed Constraints by Folder

### 5.1 api
- Hard-coding Backend endpoint URLs is prohibited; configuration must be loaded from environment variables/global config.
- API call methods must be grouped by business entity (e.g., `questions.ts`, `attempts.ts`), corresponding to the Backend controllers.
- Centralize Axios configuration in `axios.ts`, automatically injecting authorization tokens from the store and implementing a global response interceptor (Error Interceptor) to show error messages or log out users.

### 5.2 assets
- Contains shared CSS or stylesheets (such as `main.css`, custom stylesheets).
- Stylesheets must be organized cleanly; writing inline styles directly within HTML tags in Vue components is prohibited.

### 5.3 components
- Vue component filenames must be formatted in `PascalCase`.
- Must be stateless components (no internal business logic), or only handle UI logic and pass data to the parent component via `props` and `emits`.

### 5.4 models / model
- Declare clean TypeScript types (`interface` or `type`) for all request/response objects.
- Loose usage of the `any` type is strictly prohibited. Strict TypeScript configurations must be followed.
- Interface structures must align with Backend DTOs/Requests.

### 5.5 router
- Router configuration must declare meta properties clearly (e.g., `requiresAuth`, `roles`) to support access checks in navigation guards.
- Avoid writing authentication/permission checks directly in views; implement them centrally in the Router's Navigation Guards.

### 5.6 stores
- Use Pinia as the state management tool.
- Store filename and store name must match (e.g., file `auth.ts` -> store `useAuthStore`).
- Only store centralized global state (User info, tokens, global loading state, etc.). Do not abuse stores for local view-level data.

### 5.7 utils
- Contains stateless helper functions.
- Must be written in clean TypeScript and ensure helper functions are pure functions (no side-effects or external state modifications).

### 5.8 views
- View component filenames must be formatted in `PascalCase`, and are recommended to end with the suffix `View` or `Page` (e.g., `LoginView.vue`, `DashboardView.vue`).
- Organize files hierarchically by business module to ensure clean management.
- Calling APIs directly inside UI event handlers without using api helper files or stores is prohibited.
