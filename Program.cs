using chatApi.Hubs;
using chatApi.Models;
using chatApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.Configure<ContextMongoDb>(
    builder.Configuration.GetSection("ChatDatabase"));

builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<ChatTechService>();
builder.Services.AddSingleton<ChatGeekService>();
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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ChatApi",
        Description = "WebApi ASPNET, Chat with MongoDB",
        Contact = new OpenApiContact
        {
            Name = "Developer",
            Url = new Uri("https://ribshow.github.io/portfolio-react")
        },
        License = new OpenApiLicense
        {
            Name = "MIT LICENSE",
            Url = new Uri("https://github.com/ribshow/chatApi/blob/main/LICENSE")
        },
    });

    // Habilitando o caminho para o arquivo xml
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// permitindo requisi��o a partir do servidor local 
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<chatApi.Hubs.ChatTech>("/chatHubTech");
app.MapHub<chatApi.Hubs.ChatGeek>("/chatHubGeek");

app.Run();
