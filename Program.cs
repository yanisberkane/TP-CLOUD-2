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
builder.Services.Configure<ApplicationConfiguration>(builder.Configuration.GetSection("ApplicationConfiguration"));

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

// Posts

// Ajouter un post
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


// Obtenir un post
app.MapGet("/Posts/{id}", async (IRepository_mini repo, Guid id) =>
{
    return await repo.GetAPIPost(id);
});

// Obtenir les posts par page
app.MapGet("/Posts", async (IRepository_mini repo, uint pageNumber, uint pageSize) =>
{
    return await repo.GetPostsIndex((int)pageNumber, (int)pageSize);
});

// Liker un post
app.MapPost("/Posts/IncrementPostLike/{id}", async (IRepository_mini repo, Guid id) =>
{
    return await repo.IncrementPostLike(id);
});

// Disliker un post
app.MapPost("/Posts/IncrementPostDislike/{id}", async (IRepository_mini repo, Guid id) =>
{
    return await repo.IncrementPostDislike(id);
});

// Commentaires

// Ajouter un commentaire
app.MapPost("/Comments/Add", async (IRepository_mini repo, [FromForm] string PostId, [FromForm] string User, [FromForm] string Commentaire) =>
{
    try
    {
        Comment comment = new Comment { PostId = Guid.Parse(PostId), User = User, Commentaire = Commentaire };
        return await repo.AddComments(comment);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return TypedResults.BadRequest();
    }
}).DisableAntiforgery();


// Obtenir les commentaires
app.MapGet("/Comments/{id}", async (IRepository_mini repo, Guid id) =>
{
    return await repo.GetCommentsIndex(id);
});

// Liker un commentaire
app.MapPost("/Comments/IncrementCommentLike/{id}", async (IRepository_mini repo, Guid id) =>
{
    return await repo.IncrementCommentLike(id);
});

// Disliker un commentaire
app.MapPost("/Comments/IncrementCommentDislike/{id}", async (IRepository_mini repo, Guid id) =>
{
    return await repo.IncrementCommentDislike(id);
});

app.Run();


