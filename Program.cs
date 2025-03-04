//dotnet add package Microsoft.EntityFrameworkCore.InMemory
//dotnet add package Microsoft.EntityFrameworkCore

using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// BD InMemory ...
builder.Services.AddDbContext<ApplicationDbContextInMemory>(options => options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddScoped<IRepository_mini, EFRepository_mini_InMemory>();

// Ajouter le service pour Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.OperationFilter<FileUploadOperationFilter>() // Add custom operation filter
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//API 
app.MapPost("/Posts/Add", async (IRepository_mini repo,[FromForm] PostCreateDTO post) =>
{
    try
    {
        Guid guid = Guid.NewGuid();

        // Test write images
        string LocalFilePath = @"c:\Robot\" + post.Image.FileName;

        using (FileStream stream = new FileStream(LocalFilePath, FileMode.CreateNew))
        { 
            await post.Image.CopyToAsync(stream);
        }

        Post Post = new Post { Title = post.Title, Category = post.Category, User = post.User, BlobImage = guid, Url = LocalFilePath };
        return await repo.CreateAPIPost(Post);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return TypedResults.BadRequest();
    }
}).DisableAntiforgery();


app.Run();


