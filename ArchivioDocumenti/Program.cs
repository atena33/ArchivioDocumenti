using ArchivioDocumenti.BusinessLogic;
using ArchivioDocumenti.DataAccess;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La stringa di connessione 'DefaultConnection' non è stata trovata.");

builder.Services.AddScoped<IDocumentoRepository>(provider => new DocumentoRepository(connectionString));
builder.Services.AddScoped<ICategoriaRepository>(provider => new CategoriaRepository(connectionString));
builder.Services.AddScoped<IClienteRepository>(provider => new ClienteRepository(connectionString));

builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();

app.UseStaticFiles(); 

app.MapControllers();

app.Run();
