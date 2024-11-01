using chatApi.Hubs;
using chatApi.Models;
using chatApi.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.Configure<ContextMongoDb>(
    builder.Configuration.GetSection("ChatDatabase"));

builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<ContextMongoDb>();

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder.WithOrigins("http://127.0.0.1:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials())
            ;
});

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

// permitindo requisição a partir do servidor local 
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
