Create Modular Solution

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

