using Banking.Infrastructure;
using Banking.Service;
using Banking.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.Services.GetService<DatabaseInitializer>().Initialize();
app.MapGroup("/api/banking").WithOpenApi().MapAccounts();
app.Run();