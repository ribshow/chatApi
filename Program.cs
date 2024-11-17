using chatApi.Hubs;
using chatApi.Models;
using chatApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

// adicionando jwt bearer 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(static options =>
    {
        options.Audience = "http://127.0.0.1:8000";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7125",
            ValidAudience = "http://127.0.0.1:8000",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHAVESUPERSEGURACHAVESUPERSEGURA"))
        };
        // configurando o JWT para a porta de entrada dos HUB's
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = new PathString(context.HttpContext.Request.Path);

                // lista de caminhos para o hub
                var hubPaths = new[] { "/chatHub", "/chatHubGeek", "/chatHubTech", "/chatHubSci" };

                // verificando se a solicitação é para um dos hubs
                if (!string.IsNullOrEmpty(accessToken) && hubPaths.Any(hubPath => path.StartsWithSegments(new PathString(hubPath))))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// contexto do database
builder.Services.Configure<ContextMongoDb>(
    builder.Configuration.GetSection("ChatDatabase"));

// singleton para as classes de serviço
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<ChatTechService>();
builder.Services.AddSingleton<ChatGeekService>();
builder.Services.AddSingleton<ChatSciService>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub").RequireAuthorization();
app.MapHub<chatApi.Hubs.ChatTech>("/chatHubTech").RequireAuthorization();
app.MapHub<chatApi.Hubs.ChatGeek>("/chatHubGeek").RequireAuthorization();
app.MapHub<chatApi.Hubs.ChatSci>("/chatHubSci").RequireAuthorization();

app.Run();
