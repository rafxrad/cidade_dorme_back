using CidadeDorme.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários
builder.Services.AddControllers(); // Adiciona suporte a controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<SalaService>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Permite requisições do frontend
                  .AllowAnyMethod() // Permite GET, POST, PUT, DELETE
                  .AllowAnyHeader() // Permite qualquer cabeçalho
                  .AllowCredentials(); // Permite cookies/autenticação (se necessário)
        });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // Altere a porta se necessário
});

var app = builder.Build();

// Configuração para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
// Mapeia os endpoints do SignalR e dos controllers
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
