using Monit0.Core.Interfaces;
// using Monit0.Core.Models;
using Monit0.Infrastructure.Services;
using Monit0.Api.Mocks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IWorldCheckService, MockWorldCheckService>();
// builder.Services.AddScoped<IDataService, DataService>();
// builder.Services.AddScoped<IHtmlTemplateService, HtmlTemplateService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
