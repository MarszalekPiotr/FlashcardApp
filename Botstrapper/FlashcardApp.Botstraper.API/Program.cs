using DataBase;
using Flashcard.Modules.Users.Application.Logic.Abstract;
using FlashCard.Modules.Users.API;
using FlashCard.Shared.CQRS.Application.Logic;
using Microsoft.EntityFrameworkCore;// Dodano dla SqlClient
using Microsoft.EntityFrameworkCore.SqlServer; // Dodano dla rozszerzenia UseSqlServer
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using FlashcardApp.Botstraper.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Log parameter values as well

builder.Services.AddDbContext<Context>(opt => 
    opt.UseSqlServer("Server=localhost;Database=FlashCardDB;User Id=sa;Password=Elettric802037;TrustServerCertificate=True;"));

// add modules
builder.Services.AddModulesMediator();

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
