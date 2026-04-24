## Features

* [ ] Core Foundation

  * [X] Layered Architecture ( API -> Core -> Infrastructure -> Shared)
  * [X] ASP.Net Core Web API (Dotnet 10)
  * [X] Dependency Injection
  * [X] Configuration Management (appsettings.json, .env file)
  * [X] global exception handling
  * [ ] request and response logging
  * [X] Health Check Endpoints (for DB and other Services)

* [ ] Project Organization
  * [X] Solution File
  * [ ] Global Usings
  * [X] API Versioning Support
  * [X] CORS Configuration for Frontend
* [ ] Authentication & Authorization
  * [ ] Core Auth
    * [X] JWT Bearer Token (Authentication)
    * [ ] User Registration/ Login / Logout endpoints
    * [ ] Password Reset/ Password Forgot endpoints
    * [ ] Email Confirmation System
    * [ ] Refresh Token mechanism
    * [ ] Role-Base Access Control
  * [ ] Security
    * [X] Password Policy enforcement
    * [ ] account logout after multiple attempts
    * [X] secure password hashing
    * [ ] xxs protection headers
    * [ ] CSRF protection
    * [ ] Rate Limiting on auth endpoints
* [ ] Data Access & Database
  * [X] Code-First Migration
  * [X] Generic Repository Implementation
  * [X] Unit of Work Pattern
  * [ ] Database Initial Seeding
  * [ ] Soft Delete
  * [ ] Audit Fields Interface
  * [ ] Database Health Checks
* [ ] Data Features
  * [ ] Support Multiple Database Systems (MSSQL, Postgres, SQLite)
  * [ ] Connection String Management per ENV
  * [X] EF Core Configuration
  * [ ] Data Validation (FluentValidation)
  * [ ] Pagination helper
  * [ ] Filtering/Sorting
  * [ ] Bulk Operations

* [ ] API Quality & Documentation
  * [ ] API Features
    * [X] Swagger / OpenAPI
    * [ ] XML Comments
    * [ ] DTOs
    * [X] AutoMapper
    * [ ] Model Validation
    * [ ] Content Negotiation
  * [ ] API Management
    * [ ] Request Standardization

* [ ] Core Services
  * [ ] Essentials
    * [ ] Email Service (smtp)
    * [ ] File Storage Service (local + cloud)
    * [ ] Caching (Memory Cache + Redis)
    * [ ] Background Job Service
    * [ ] DateTime Service
    * [ ] Current User Service
  * [ ] External Integrations
    * [ ] Payment Processing
    * [ ] Cloud Storage
    * [ ] SMS Service (optional)
    * [ ] Push Notifications
    * [ ] Social Auth
* [ ] Testing & Quality
  * [ ] Test Coverage
    * [ ] Unit Testing
    * [ ] Integration Test
    * [ ] Test Data Builds
    * [ ] Database Testing
* [ ] Devops & Deployment
  * [ ] Containerization
    * [ ] Dockerfile
    * [ ] docker compose
    * [ ] docker health checker
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

## Phase Programming

* [ ] Phase 1
  * [ ] Architecture (Base API calls possible)
  * [ ] Database Setup
  * [ ] Basic Auth + Endpoint Protection
  * [ ] API Quality
* [ ] Phase 2
  * [ ] Email
  * [ ] File Storage
  * [ ] Caching
  * [ ] Testing
* [ ] Phase 3
  * [ ] Multi Tenancy
  * [ ] Payments
  * [ ] Realtime
* [ ] Phase 4
  * [ ] Admin Panel (Tenancy Overall)
  * [ ] Admin Panel (per Tenant)
  * [ ]



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
Model							//Sets Props of Object -> Inherit BaseModel
Add to DBContext		
Create Migrations and Migrate	// dotnet ef migrations add AddStorage
								// dotnet dotnet ef database update dotnet wat
InterfaceRepository				// Gives play rules for repository (model controller)
Repository						// Model Controller Inherits from InterfaceRepository
Add Repositories to Program CS 	//builder.Services.AddScoped<ITodoRepository, TodoRepository>();
DTOs							// Create DTO(return Object on regular request)
	Create DTO					// Create Create DTO  (DTO presented by Frontend for create Object)
	Update DTO					// Create Update DTO  (DTO presented by Frontend for update Object)
Create Mapper Model - DTOs
Controller						// API Entry point, inherit BaseAPIController

