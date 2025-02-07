using CidadeDorme.Hubs;
using CidadeDorme.Services;

var builder = WebApplication.CreateBuilder(args);

// Definição da política de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:5073", "http://frontend") // Permite requisições locais e do Docker
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// Adiciona os serviços necessários
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<SalaService>();

var app = builder.Build();

// Configuração para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Evita erro de HTTPS no Docker
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Aplica a política de CORS antes do roteamento
app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();
app.UseAuthorization();

// Mapeia os endpoints do SignalR e dos controllers
app.MapControllers();
app.MapHub<JogoHub>("/jogohub");

app.Run();
