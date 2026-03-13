using Microsoft.EntityFrameworkCore;
using WhatToCook.DAL.Data;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Спочатку додаємо всі сервіси (ДО builder.Build())
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Отримуємо рядок підключення
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // РЕЄСТРУЄМО КОНТЕКСТ ТУТ
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // 2. Створюємо додаток (ТІЛЬКИ ПІСЛЯ реєстрації всіх сервісів)
        var app = builder.Build();

        // 3. Налаштовуємо Middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}