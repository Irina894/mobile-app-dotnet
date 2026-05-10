using AutoMapper;
using WhatToCook.BLL.DTOs.Recipe;
using WhatToCook.BLL.DTOs.User;
using WhatToCook.BLL.DTOs.Ingredient;
using WhatToCook.DAL.Entities;

namespace WhatToCook.BLL.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── 1. Рецепти ────────────────────────────────────────────────────

        // Entity → DTO: CookingTime (БД) → CookTimeMinutes (API відповідь / MAUI)
        CreateMap<Recipe, RecipeDto>()
            .ForMember(dest => dest.CookTimeMinutes,
                       opt => opt.MapFrom(src => src.CookingTime));

        // CreateRecipeDto → Entity: CookTimeMinutes (з форми) → CookingTime (БД)
        CreateMap<CreateRecipeDto, Recipe>()
            .ForMember(dest => dest.CookingTime,
                       opt => opt.MapFrom(src => src.CookTimeMinutes))
            .ForMember(dest => dest.Rating, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.FavoriteRecipes, opt => opt.Ignore());

        // UpdateRecipeDto → Entity: аналогічно
        CreateMap<UpdateRecipeDto, Recipe>()
            .ForMember(dest => dest.CookingTime,
                       opt => opt.MapFrom(src => src.CookTimeMinutes))
            .ForMember(dest => dest.Rating, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.FavoriteRecipes, opt => opt.Ignore());

        // ── 2. Користувачі ────────────────────────────────────────────────
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // ── 3. Інгредієнти (Довідник) ─────────────────────────────────────
        CreateMap<Ingredient, IngredientDto>().ReverseMap();
        CreateMap<CreateIngredientDto, Ingredient>();
        CreateMap<UpdateIngredientDto, Ingredient>();

        // ── 4. Склад рецепта (RecipeIngredient) ───────────────────────────
        CreateMap<RecipeIngredient, RecipeIngredientDto>()
            .ForMember(dest => dest.IngredientName,
                       opt => opt.MapFrom(src => src.Ingredient != null
                           ? src.Ingredient.Name
                           : "Невідомо"));

        // ── 5. Обрані рецепти (Favorites) ─────────────────────────────────
        CreateMap<FavoriteRecipe, FavoriteRecipeDto>()
            .ForMember(dest => dest.RecipeTitle,
                       opt => opt.MapFrom(src => src.Recipe != null
                           ? src.Recipe.Title
                           : "Рецепт видалено"))
            .ForMember(dest => dest.RecipeImageUrl,
                       opt => opt.MapFrom(src => src.Recipe != null
                           ? src.Recipe.ImageUrl
                           : null));

        CreateMap<CreateFavoriteRecipeDto, FavoriteRecipe>();
    }
}