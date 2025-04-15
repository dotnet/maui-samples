var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<TodoAPI.Interfaces.ITodoRepository, TodoAPI.Services.TodoRepository>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
