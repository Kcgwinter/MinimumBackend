## Features

* [ ] Core Foundation

  * [ ] Layered Architecture ( API -> Core -> Infrastructure -> Shared)
  * [X] ASP.Net Core Web API (Dotnet 10)
  * [ ] Dependency Injection
  * [ ] Configuration Management (appsettings.json, .env file)
  * [ ] global exception handling
  * [ ] request and response logging
  * [ ] Health Check Endpoints (for DB and other Services)
* [ ] Project Organization

  * [ ] Solution File
  * [ ] Global Usings
  * [ ] API Versioning Support
  * [ ] CORS Configuration for Frontend
* [ ] Authentication & Authorization

  * [ ] Core Auth

    * [ ] JWT Bearer Token (Authentication)
    * [ ] User Registratio/ Login / Logout endpoints
    * [ ] Password Reset/ Password Forgot endpoints
    * [ ] Email Confirmation System
    * [ ] Refresh Token mechanism
    * [ ] Role-Base Access Control
  * [ ] Security

    * [ ] Password Policy enforcement
    * [ ] account logout after multiple attemps
    * [ ] secure password hashing
    * [ ] xxs protection headers
    * [ ] CSRF protection
    * [ ] Rate Limiting on auth endpoints
* [ ] Data Access & Database

  * [ ] Code-First Migration
  * [ ] Generic Repository Implementation
  * [ ] Unit of Work Pattern
  * [ ] Database Initial Seeding
  * [ ] Soft Delete
  * [ ] Audit Fields Interface
  * [ ] Database Health Checks
* [ ] Data Features

  * [ ] Support Multiple Database Systems (MSQL, Postgres, SQLite)
  * [ ] Connection String Management per ENV
  * [ ] EF Core Configuration
  * [ ] Data Validation (FluentValidation)
  * [ ] Pagination helper
  * [ ] Filtering/Sorting
  * [ ] Bulk Operations
* [ ] API Quatiliy & Documentation

  * [ ] API Features
    * [ ] Swagger / OpenAPI
    * [ ] XML Comments
    * [ ] DTOs
    * [ ] AutoMapper
    * [ ] Model Validation
    * [ ] Content Negotiation
  * [ ] API Managment
    * [ ] Request Standardization
* [ ] Core Services

  * [ ] Essentials
    * [ ] Email Service (smtp)
    * [ ] File Storage Service (local + cloud)
    * [ ] Caching (Memory Cache + Redis)
    * [ ] Background Job Service
    * [ ] DateTime Service
    * [ ] Current User Service
  * [ ] External Intergrations
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
    * [ ] Tentant Identification (header, subdomain)
    * [ ] Data Isolation
    * [ ] Tenent Menagement
    * [ ] Tenant specific configs
  * [ ] Billing & Subscription
    * [ ] Stripe for Payments
    * [ ] Subscription Management
    * [ ] Plan/ Features Managemnt
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
  * [ ] Multi Tenacy
  * [ ] Payments
  * [ ] Realtime
* [ ] Phase 4
  * [ ] Admin Panel (Tenance Overall)
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
								// dotnet dotnet ef database updatedotnet wat
InterfaceRepository				// Gives playrules for repository (model controller)
Repository						// Model Controller Inherits from InterfaceRepository
Add Repositories to Program CS 	//builder.Services.AddScoped<ITodoRepository, TodoRepository>();
DTOs							// Create DTO(return Object on regular request)
	Create DTO					// Create Create DTO  (DTO presented by Frontend for create Object)
	Update DTO					// Create Update DTO  (DTO presended by Frontend for update Object)
Create Mapper Model - DTOs
Controller						// API Entry point, inherit BaseAPIController

