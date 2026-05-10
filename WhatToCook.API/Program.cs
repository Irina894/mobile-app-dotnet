using Microsoft.EntityFrameworkCore;
using WhatToCook.BLL.Interfaces;
using WhatToCook.BLL.Profiles;
using WhatToCook.BLL.Services;
using WhatToCook.DAL.Data;
using WhatToCook.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Стандартні сервіси API ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── 2. CORS — обов'язково для MAUI на Android/iOS ─────────────────────────
// Без цього MAUI буде отримувати помилку мережі навіть якщо API працює
builder.Services.AddCors(options =>
{
    options.AddPolicy("MauiPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── 3. База даних (SQLite) ─────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// ── 4. Репозиторій (DAL) ───────────────────────────────────────────────────
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ── 5. AutoMapper ──────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, typeof(Program));

// ── 6. Сервіси BLL ────────────────────────────────────────────────────────
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// ── 7. Побудова додатку ───────────────────────────────────────────────────
var app = builder.Build();

// ── 8. Автоматична міграція + Seed при старті ─────────────────────────────
// Це гарантує що таблиці та тестові дані будуть створені автоматично
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Застосовує всі міграції включно з Seed даними
}

// ── 9. Middleware ──────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS обов'язково ДО UseAuthorization
app.UseCors("MauiPolicy");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();