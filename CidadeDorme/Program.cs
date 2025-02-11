using CidadeDorme.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<SalaService>();

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


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Altere a porta se necessário
});

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


app.UseCors(MyAllowSpecificOrigins);
// Mapeia os endpoints do SignalR e dos controllers
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

