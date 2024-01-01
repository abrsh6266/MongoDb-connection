using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDb.Models;
using MongoDb.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// MongoDB services
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDBService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/playlists", async (MongoDBService mongoDBService) =>
{
    return await mongoDBService.GetAsync();
})
.WithName("GetPlaylists")
.WithOpenApi();

app.MapPost("/api/playlists", async (MongoDBService mongoDBService, Playlist playlist) =>
{
    await mongoDBService.CreateAsync(playlist);
    return Results.Created($"/api/playlists/{playlist.Id}", playlist);
})
.WithName("CreatePlaylist")
.WithOpenApi();

app.MapPut("/api/playlists/{id}/add", async (MongoDBService mongoDBService, string id, [FromBody] string movieId) =>
{
    await mongoDBService.AddToPlaylistAsync(id, movieId);
    return Results.NoContent();
})
.WithName("AddToPlaylist")
.WithOpenApi();

app.MapDelete("/api/playlists/{id}", async (MongoDBService mongoDBService, string id) =>
{
    await mongoDBService.DeleteAsync(id);
    return Results.NoContent();
})
.WithName("DeletePlaylist")
.WithOpenApi();

app.Run();
