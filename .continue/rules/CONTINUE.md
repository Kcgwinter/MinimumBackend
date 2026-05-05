# Project Guide: CONTINUE.md

## 💡 Project Overview
This project appears to be a backend application, likely an API service, built using **C#** and **.NET**. It follows a layered architecture typical of enterprise applications, separating core domain logic, data access/infrastructure, and the presentation/API layer.

**Purpose:**
To provide a robust, secure, and data-driven service (e.g., user management, authentication, resource control) that exposes RESTful APIs.

**Key Technologies Used:**
*   **Language:** C#
*   **Framework:** .NET (Implied .NET Core, given file structures)
*   **Architecture:** Layered Architecture (Core, Infrastructure, Application/API)
*   **Database:** Entity Framework Core (Evidenced by `AppDbContext`, `Migrations` folders, and connection strings/store files).
*   **Security/Features:** Role-Based Access Control (RBAC), Authentication, Middleware (CSRF, Logging).

**High-Level Architecture:**
The system uses three primary components:
1.  **`Core`:** Contains the fundamental business entities (`User`, `Role`, `Permission`), Data Transfer Objects (DTOs), and business logic validators. It is the heart of the domain model.
2.  **`Infrastructure`:** Handles all persistence logic. This layer contains the `DbContext`, manages database migrations, and implements repository patterns (`IPermissionProvider`, etc.) to interact with the actual database store.
3.  **`Api`:** This is the entry point. It hosts the API controllers (`AuthController`, `RBACTestController`) and middleware (`LoggingMiddleware`, `ExceptionMiddleware`) that process incoming HTTP requests, utilize services, and return responses.
4.  **`Application`:** Contains higher-level services (e.g., `AuthService`, `EmailService`) that orchestrate calls between `Core` and `Infrastructure` to complete business workflows.

---

## 🚀 Getting Started

### Prerequisites
*   .NET SDK (Must be compatible with the project's target framework, e.g., .NET 6, 7, or 8, based on build outputs).
*   A database system compatible with Entity Framework Core (e.g., SQLite, SQL Server, etc., based on the `store.db` files).
*   A code editor supporting C# development (e.g., Visual Studio or VS Code with C# extensions).

### Installation Instructions
1.  **Clone the Repository:**
    ```bash
    git clone <repository-url>
    cd <repository-name>
    ```
2.  **Restore Dependencies:** Run the following command from the root directory to ensure all NuGet packages are downloaded and restored for the solution:
    ```bash
    dotnet restore
    ```
3.  **Database Setup (Migrations):** The database schema must be created and updated. You will likely need to run migrations against the `Infrastructure` project first.
    *(Assumption: The correct command for applying migrations needs verification. It usually involves targeting the startup project or the Infrastructure project.)*
    **Suggested Command:**
    ```bash
    dotnet ef database update --project Infrastructure --startup-project Api
    ```

### Basic Usage Examples
*   **Authentication:** Access the API endpoints defined in `Api/Controllers/AuthController.cs` (e.g., login, register).
*   **Authorization:** Use endpoints that require specific permissions, testing the custom attributes like `[HasPermission(typeof(SomeFeature))]`.

### Running Tests
Tests are likely located in a dedicated test project (not visible in the current file list, but assumed).
*   **Suggested Command:**
    ```bash
    dotnet test
    ```

---

## 📂 Project Structure

*   **`Core/`:**
    *   **Role:** The domain model layer. It defines the *what* (entities, DTOs, validators).
    *   **Key Files:** `User.cs`, `Role.cs`, `Permission.cs`, `BaseEntity.cs`.
*   **`Infrastructure/`:**
    *   **Role:** The data access layer. It defines *how* data is persisted and accessed.
    *   **Key Files:** `AppDbContext.cs` (The primary context), Migration files (e.g., `*Create*...cs`), `IPermissionProvider.cs` (Interface for permission logic).
*   **`Application/`:**
    *   **Role:** The orchestration layer. It contains services that execute complex business workflows, mediating between Core and Infrastructure.
    *   **Key Files:** `AuthService.cs`, `EmailService.cs`.
*   **`Api/`:**
    *   **Role:** The presentation/entry point layer. It exposes functionality via HTTP endpoints.
    *   **Key Files:** `Program.cs` (Application startup configuration), `AuthController.cs` (API Controllers), `Middleware/` (Cross-cutting concerns).
*   **`MinimunBackend.sln`:** The solution file, orchestrating all projects.

---

## 🛠️ Development Workflow

### Coding Standards & Conventions
*   **Naming:** C# standard conventions (PascalCase for types/methods, camelCase for local variables).
*   **Immutability:** Consider using `record` types where appropriate in `Core` for DTOs to enforce immutability.
*   **Service Composition:** Services in `Application` should accept dependencies via constructor injection.

### Testing Approach
*   **Unit Testing:** Focus on `Core` entities and `Application` services in isolation. Mocks should be used extensively for database/repository interactions.
*   **Integration Testing:** Use EF Core in-memory databases or test containers to verify the full stack interaction between `Api`, `Application`, and `Infrastructure`.

### Build and Deployment Process
1.  **Building:** Building the solution should be done from the root directory:
    ```bash
    dotnet build --configuration Release
    ```
2.  **Deployment:** The final deployable artifact is likely contained within the `Api/bin/Release/netX.Y/` folder.
3.  **Database Migration (Crucial Step):** Always run database migrations *before* deploying the API to ensure schema consistency.

### Contribution Guidelines
*   When adding new features, start by defining the necessary **Entities** in `Core`, then define the **Repository/Interface** in `Infrastructure`, and finally implement the **Workflow** in `Application`, exposing it via a **Controller** in `Api`.

---

## 🧩 Key Concepts

*   **Repository Pattern:** Used via `Core\Interfaces\IRepository.cs` and specialized providers. It abstracts the data source logic away from the business logic.
*   **Middleware:** Used in the `Api` layer (`LoggingMiddleware`, `ExceptionMiddleware`) to intercept and modify HTTP requests/responses globally.
*   **RBAC (Role-Based Access Control):** Core security mechanism managing `User`, `Role`, and `Permission` relationships.
*   **DbContext:** The primary interface between the application code and the relational database schema.

---

## ⚙️ Common Tasks

*   **Creating a new resource/endpoint:**
    1.  Define new **Entities/DTOs** in `Core`.
    2.  Update **Migrations** in `Infrastructure` and run the update.
    3.  Implement the transactional **Service Logic** in `Application`.
    4.  Add a new **Controller Endpoint** in `Api`.
*   **Updating Business Logic:** Modify the relevant service in `Application` and add corresponding unit tests in the Test project.

---

## ⚠️ Troubleshooting

*   **Error: NullReferenceException in API:** Check the request payload structure and the validation logic in the DTO validators (`Core\Validators\...`).
*   **Error: Missing Database Migration:** Rerun the `dotnet ef database update` command. If it fails, inspect the last successful migration files.
*   **Error: `System.InvalidOperationException` related to context:** This usually means the service trying to access the context does not have the correct scope or the database connection string in `appsettings.json` is incorrect.

---

## 📚 References

*   **Source Code:** The file system itself serves as documentation.
*   **API Contract:** Review `Api/Api.http` for expected endpoints and request/response bodies.

***

**Note:** Many sections, particularly those marked with *Assumption* or *Suggested*, are educated guesses based on common .NET patterns. Please **thoroughly review and update** this document based on actual internal standards, team conventions, and specific framework versions used.