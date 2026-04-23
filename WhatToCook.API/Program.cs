using Microsoft.EntityFrameworkCore;
using WhatToCook.BLL.Interfaces;
using WhatToCook.BLL.Profiles;
using WhatToCook.BLL.Services;
using WhatToCook.DAL.Data;
using WhatToCook.DAL.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- 1. Налаштування стандартних сервісів API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. Налаштування бази даних (DAL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// --- 3. Реєстрація Репозиторію (DAL) ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


// --- 4. Налаштування AutoMapper (Твій Блок 3) ---
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, typeof(Program));

// --- 5. Реєстрація ТВОЇХ Сервісів (Твій Блок 3 - BLL) ---
// Кожен інтерфейс тепер зв'язаний зі своїм класом-сервісом
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// --- 6. Побудова додатку ---
var app = builder.Build();

// --- 7. Налаштування Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();