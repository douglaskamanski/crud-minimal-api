using crud_minimal_api.Data;
using crud_minimal_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<User>> GetUsers(AppDbContext context)
{
    return await context.Users.ToListAsync();
}

//Read all
app.MapGet("/Users", async (AppDbContext context) =>
{
    return await GetUsers(context);
});

//Read by id
app.MapGet("Users/{id}", async (AppDbContext context, int id) =>
{
    var user = await context.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound("User not found");
    }

    return Results.Ok(user);
});

//Create
app.MapPost("/User", async (AppDbContext context, User user) =>
{
    context.Users.Add(user);
    await context.SaveChangesAsync();

    return await GetUsers(context);
});

//Update
app.MapPut("/User", async (AppDbContext context, User user) =>
{
    var userDb = await context.Users.FindAsync(user.Id);

    if (userDb == null) return Results.NotFound("User not found");

    userDb.Username = user.Username;
    userDb.Email = user.Email;
    userDb.Name = user.Name;

    context.Update(userDb);
    await context.SaveChangesAsync();

    return Results.Ok(await GetUsers(context));
});

//Delete
app.MapDelete("/User/{id}", async (AppDbContext context, int id) =>
{
    var userDb = await context.Users.FindAsync(id);

    if (userDb == null) return Results.NotFound("User not found");

    context.Users.Remove(userDb);
    await context.SaveChangesAsync();

    return Results.Ok(await GetUsers(context));
});

app.Run();
