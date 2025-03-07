//dotnet add package Microsoft.EntityFrameworkCore.InMemory
//dotnet add package Microsoft.EntityFrameworkCore

using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Data;
using Microsoft.AspNetCore.Mvc;
using MVC.Business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// BD InMemory ...
builder.Services.AddDbContext<ApplicationDbContextInMemory>(options => options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddScoped<IRepository_mini, EFRepository_mini_InMemory>();

// Configure ApplicationConfiguration
//builder.Services.Configure<ApplicationConfiguration>(builder.Configuration.GetSection("ApplicationConfiguration"));

// Ajouter le service pour BlobController
builder.Services.AddScoped<BlobController>();

// Ajouter le service pour Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<FileUploadOperationFilter>(); // Ajoute le filtre custom (déjà présent)
});

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
app.MapPost("/Posts/Add", async (IRepository_mini repo, IFormFile Image, HttpRequest request, BlobController blob) =>
{
    try
    {
        PostCreateDTO post = new PostCreateDTO(request.Form["Title"]!, request.Form["Category"]!, request.Form["User"]!, Image);
        Guid guid = Guid.NewGuid();

        string Url = await blob.PushImageToBlob(post.Image!, guid);

        Post Post = new Post { Title = post.Title!, Category = post.Category, User = post.User!, BlobImage = guid, Url = Url };

        return await repo.CreateAPIPost(Post);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return TypedResults.BadRequest();
    }
}).DisableAntiforgery();


app.Run();


