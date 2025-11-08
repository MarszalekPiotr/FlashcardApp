using DataBase;
using FlashCard.Modules.Users.API;
using Microsoft.EntityFrameworkCore;// Dodano dla SqlClient
using Microsoft.EntityFrameworkCore.SqlServer; // Dodano dla rozszerzenia UseSqlServer
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Log parameter values as well

builder.Services.AddDbContext<Context>(opt => 
    opt.UseSqlServer("Data Source=vocabulary.db"));


// add modules
builder.Services.AddUserModule();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
