//======program.cs===========
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // כתובת ה-URL של ה-React frontend
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    });
}
// Enable CORS

app.UseCors("AllowReactApp");


app.MapGet("/api/todos", async (ToDoDbContext context) =>
{
    return await context.Items.ToListAsync();
});

app.MapPost("/api/todos", async (ToDoDbContext context, Item todo) =>
{
    context.Items.Add(todo);
    await context.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapPut("/api/todos/{id}", async (ToDoDbContext context, int id, Item updatedTodo) =>
{
    // Fetch the existing entity from the database
    var existingTodo = await context.Items.FindAsync(id);
    
    if (existingTodo == null)
    {
        return Results.NotFound();
    }

    // Update the properties of the existing entity
    existingTodo.IsComplete = updatedTodo.IsComplete;
    try
    {
        // Save changes
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (DbUpdateConcurrencyException)
    {
        // Handle concurrency conflicts if needed
        return Results.BadRequest("Concurrency conflict occurred.");
    }
});

app.MapDelete("/api/todos/{id}", async (ToDoDbContext context, int id) =>
{
    var todo = await context.Items.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }

    context.Items.Remove(todo);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();