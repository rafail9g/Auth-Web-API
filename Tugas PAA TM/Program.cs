using Tugas_PAA_TM.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Pertanian API", Version = "v1" });
});

// Dependency Injection — IConfiguration otomatis tersedia, tidak perlu DatabaseConnection
builder.Services.AddScoped<LahanRepository>();
builder.Services.AddScoped<TanamanRepository>();
builder.Services.AddScoped<PanenRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();