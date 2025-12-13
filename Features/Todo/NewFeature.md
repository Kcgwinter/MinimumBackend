
## Step by Step

### Structure in the Features folder:
MyApi/Features/Todo/
├── Controllers/
│   └── TodoController.cs
├── Models/
│   └── TodoItem.cs
├── Services/
│   └── TodoService.cs
├── Data/
│   └── TodoDbContext.cs
└── TodoListExtensions.cs  # For clean Program.cs integration


### 


## DB Migrations
To run DB Migrations run in the Main API:

dotnet ef migrations add InitialCreate --project ..\Features\Todo --startup-project . --context TodoDbContext
dotnet ef database update --context TodoDbContext

To add the feature to the Main API add in the Program.cs the following lines:

builder.Services.AddTodoFeature(builder.Configuration.GetConnectionString("DefaultConnection"));

Then run the following command to add the feature:
...
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var dbTodoContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();  //<-- add this line
    dbContext.Database.EnsureCreated();  
    dbTodoContext.Database.EnsureCreated();                                         //<-- add this line
}
...

you need to register the context in the csproject of the api:

  <ItemGroup>
    <ProjectReference Include="..\Application\Application.csproj" />
    <ProjectReference Include="..\Core\Core.csproj" />
    <ProjectReference Include="..\Features\Todo\Todo.csproj" />                     //<== add this line
  </ItemGroup>

