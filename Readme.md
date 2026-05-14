## Features

* [X] Core Foundation
  * [X] Layered Architecture ( API -> Core -> Infrastructure -> Shared)
  * [X] ASP.Net Core Web API (Dotnet 10)
  * [X] Dependency Injection
  * [X] Configuration Management (appsettings.json, .env file)
  * [X] global exception handling
  * [X] request and response logging
  * [X] Health Check Endpoints (for DB and other Services)
* [X] Project Organization
  * [X] Solution File
  * [-] Global Usings //Not Implemented in 1.0, reason Validation which usings are really needed
  * [X] API Versioning Support
  * [X] CORS Configuration for Frontend
* [X] Authentication & Authorization
  * [X] Core Auth
    * [X] JWT Bearer Token (Authentication)
    * [X] User Registration/ Login / Logout endpoints
    * [X] Password Reset/ Password Forgot endpoints
    * [X] Email Confirmation System
    * [X] Refresh Token mechanism
    * [X] Role-Base Access Control (RBAC)
  * [X] Security
    * [X] Password Policy enforcement
    * [X] secure password hashing
    * [X] CORS protection headers
    * [X] CSRF protection
    * [X] Rate Limiting on auth endpoints --> AspNetCoreRateLimit
* [X] Data Access & Database
  * [X] Code-First Migration
  * [X] Generic Repository Implementation
  * [X] Unit of Work Pattern (Removing Unit Of Work -> Ef Core already Abstracting)
  * [X] Soft Delete
  * [X] Audit Fields Interface
  * [X] Database Health Checks
* [ ] Data Features
  * [?] Support Multiple Database Systems (MSSQL, Postgres, SQLite) [pushed to future versions]
  * [X] Connection String Management per AppSettings (AppSettings.dev)
  * [X] EF Core Configuration
  * [X] Data Validation (FluentValidation)
  * [X] DBInitializer
* [X] API Quality & Documentation
  * [X] API Features
    * [X] Swagger / OpenAPI
    * [X] DTOs
    * [X] AutoMapper
    * [X] Model Validation -> used FluentValidation
    * [X] Content Negotiation -> Later Implementation to prevent Early Overhead
* [?] Testing & Quality
  * [?] Test Coverage
    * [X] Unit Testing
    * [ ] Integration Test
    * [ ] Test Data Builds
    * [ ] Database Testing
* [ ] Devops & Deployment
  * [ ] Containerization
    * [ ] Dockerfile
    * [ ] docker compose
    * [ ] docker health checker
* [] Important Features to implement
  * [] MediatR / CQRS
  * [] Domain Events
  * [] MediatR / CQRS



Later Versions:
* [ ] SAAS Features
  * [ ] Multi-Tenancy
    * [ ] Tenant Identification (header, subdomain)
    * [ ] Data Isolation
    * [ ] Tenant Management
    * [ ] Tenant specific configs
  * [ ] Billing & Subscription
    * [ ] Stripe for Payments
    * [ ] Subscription Management
    * [ ] Plan/ Features Management
    * [ ] Usage Tracking
    * [ ] Invoice Generation
* [ ] Core Services
  * [ ] Essentials
    * [X] Email Service (smtp)
    * [ ] File Storage Service (local + cloud)
    * [ ] Caching (Memory Cache + Redis)
    * [ ] Background Job Service
    * [ ] DateTime Service
    * [ ] Current User Service


//Create Solution File
dotnet new sln

// Add Web
dotnet new web -o Host
dotnet sln .\ModularMonolith.sln add .\Host\Host.csproj 

// Create Modules
dotnet new classlib -o Modules/Module1
// Add Module to Solution
dotnet sln .\ModularMonolith.sln add .\Modules\Module1\Module1.csproj

//Add AspNetCore.App Framework Reference to Modules
//via Nuget Package to each module project
microsoft.aspnetcore.app 


## To Add a new Feature:
Follow the following steps:

- Model							//Sets Props of Object -> Inherit BaseModel
- Add to DBContext
- Create Migrations and Migrate	// dotnet ef migrations add AddStorage
- InterfaceRepository				// Gives play rules for repository (model controller)
- Repository						// Model Controller Inherits from InterfaceRepository
- Add Repositories to Program CS 	//builder.Services.AddScoped<ITodoRepository, TodoRepository>();
- DTOs							// Create DTO(return Object on regular request)
	- Create DTO					// Create Create DTO  (DTO presented by Frontend for create Object)
	- Update DTO					// Create Update DTO  (DTO presented by Frontend for update Object)
- Create Mapper Model - DTOs
- Controller						// API Entry point, inherit BaseAPIController


## DB Migrations
dotnet ef database update --project ./Infrastructure --startup-project ./Api --context AppDbContext

dotnet ef migrations add AuthUpdate --project ./Infrastructure --startup-project ./Api --context AppDbContext 