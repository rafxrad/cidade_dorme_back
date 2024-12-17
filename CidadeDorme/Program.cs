using CidadeDorme.Hubs;
using CidadeDorme.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários
builder.Services.AddControllers(); // Adiciona suporte a controllers
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

app.UseHttpsRedirection();

// Mapeia os endpoints do SignalR e dos controllers
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHub<JogoHub>("/jogohub");

app.Run();
