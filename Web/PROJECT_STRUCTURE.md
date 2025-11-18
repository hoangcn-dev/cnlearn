# Angular Base Project

A well-structured Angular application following best practices with standalone components.

## 📁 Project Structure

```
src/
│
├── app/
│   ├── core/                      # Core functionality for the entire application
│   │   ├── guards/                # Route guards (AuthGuard)
│   │   │   └── auth.guard.ts
│   │   ├── interceptors/          # HTTP interceptors
│   │   │   ├── auth.interceptor.ts
│   │   │   └── error.interceptor.ts
│   │   ├── services/              # Application-wide services
│   │   │   ├── auth.service.ts
│   │   │   └── api.service.ts
│   │   ├── models/                # Shared interfaces and models
│   │   │   ├── user.model.ts
│   │   │   └── api-response.model.ts
│   │   └── core.module.ts         # Core module (singleton)
│   │
│   ├── shared/                    # Reusable components, directives, and pipes
│   │   ├── components/
│   │   │   ├── loading-spinner/
│   │   │   └── button/
│   │   ├── directives/
│   │   │   ├── highlight.directive.ts
│   │   │   └── has-permission.directive.ts
│   │   ├── pipes/
│   │   │   ├── truncate.pipe.ts
│   │   │   └── time-ago.pipe.ts
│   │   └── shared.module.ts
│   │
│   ├── features/                  # Feature modules
│   │   ├── dashboard/
│   │   │   ├── components/
│   │   │   │   └── stats-card/
│   │   │   ├── pages/
│   │   │   │   └── dashboard-overview/
│   │   │   ├── services/
│   │   │   │   └── dashboard.service.ts
│   │   │   └── dashboard.routes.ts
│   │   └── user/
│   │       ├── components/
│   │       │   └── user-card/
│   │       ├── pages/
│   │       │   └── user-list/
│   │       ├── services/
│   │       │   └── user.service.ts
│   │       └── user.routes.ts
│   │
│   ├── app.routes.ts              # Main routing configuration
│   ├── app.component.ts|html|css  # Root component
│   └── app.config.ts              # Global providers configuration
│
├── assets/                        # Static files
│   ├── images/
│   ├── icons/
│   └── config.json
│
├── environments/                  # Environment configurations
│   ├── environment.ts             # Development environment
│   └── environment.prod.ts        # Production environment
│
├── main.ts                        # Application entry point
└── styles.css                     # Global styles

```

## 🚀 Features

### Core Module
- **Guards**: Route protection with AuthGuard
- **Interceptors**: HTTP interceptors for authentication and error handling
- **Services**: 
  - `AuthService`: Authentication and authorization
  - `ApiService`: HTTP API wrapper
- **Models**: Shared TypeScript interfaces

### Shared Module
- **Components**:
  - `LoadingSpinnerComponent`: Loading indicator
  - `ButtonComponent`: Reusable button with variants
- **Directives**:
  - `HighlightDirective`: Element highlighting on hover
  - `HasPermissionDirective`: Permission-based visibility
- **Pipes**:
  - `TruncatePipe`: Text truncation
  - `TimeAgoPipe`: Human-readable time differences

### Features
- **Dashboard**: Overview page with statistics
- **User Management**: User list and management

## 🛠️ Getting Started

### Installation
\`\`\`bash
npm install
\`\`\`

### Development Server
\`\`\`bash
npm start
\`\`\`
Navigate to `http://localhost:4200/`

### Build
\`\`\`bash
npm run build
\`\`\`

### Running Tests
\`\`\`bash
npm test
\`\`\`

## 📝 Architecture Guidelines

### Standalone Components
This project uses Angular standalone components (no NgModules required for most features).

### Lazy Loading
Feature modules are lazy-loaded using the router for better performance.

### Service Injection
Services use the new `inject()` function for dependency injection.

### Environment Configuration
Use environment files to manage different configurations for development and production.

## 🔐 Security

- HTTP interceptors handle authentication tokens
- Route guards protect sensitive routes
- Error interceptor provides centralized error handling

## 📦 Adding New Features

1. Create a new folder under `src/app/features/`
2. Add components, pages, and services
3. Create a `.routes.ts` file for routing
4. Register the routes in `app.routes.ts`

## 🎨 Styling

Global styles are in `src/styles.css`. Component-specific styles use scoped CSS.

## 🤝 Contributing

1. Follow the established folder structure
2. Use standalone components
3. Keep services in `providedIn: 'root'`
4. Document complex logic
5. Write unit tests for new features

## 📄 License

MIT
