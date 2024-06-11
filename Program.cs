using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using EconoMe_API.Data;
using EconoMe_API.Services;
using EconoMe_API.Services.IServices;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Caminho para o arquivo serviceAccountKey.json
var serviceAccountPath = Path.Combine(AppContext.BaseDirectory, "Secrets", "serviceAccountKey.json");

// Verificar se o arquivo existe
if (!File.Exists(serviceAccountPath))
{
    throw new FileNotFoundException("O arquivo serviceAccountKey.json não foi encontrado.", serviceAccountPath);
}

// Inicializar Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(serviceAccountPath)
});

// Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("AzureConnectionString");
builder.Services.AddDbContext<DataContext>(opts =>
{
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    opts.UseMySql(connectionString, serverVersion);
});

// Configuração dos serviços
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// Configuração dos controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EconoMe_API", Version = "v1" });

    // Ajustar o caminho do arquivo XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Verificar se o arquivo XML existe
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        // Logar um aviso se o arquivo XML não for encontrado
        Console.WriteLine($"Aviso: O arquivo de documentação XML '{xmlPath}' não foi encontrado.");
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "Cabeçalho de autorização JWT usando o esquema Bearer" +
                      "Insira 'Bearer' [espaço] e depois seu token no campo de texto abaixo." +
                      "Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EconoMe_API v1");
});

app.UseHttpsRedirection();

// Adicione o middleware de autenticação do Firebase diretamente ao pipeline
app.UseMiddleware<FirebaseAuthenticationMiddleware>();

// Adicionar o middleware CORS
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();