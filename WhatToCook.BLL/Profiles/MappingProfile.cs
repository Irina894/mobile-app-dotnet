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
        // 1. Рецепти
        CreateMap<Recipe, RecipeDto>().ReverseMap();
        CreateMap<CreateRecipeDto, Recipe>();
        CreateMap<UpdateRecipeDto, Recipe>();

        // 2. Користувачі
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // 3. Інгредієнти (Довідник)
        CreateMap<Ingredient, IngredientDto>().ReverseMap();
        CreateMap<CreateIngredientDto, Ingredient>();
        CreateMap<UpdateIngredientDto, Ingredient>();

        // 4. Склад рецепта (RecipeIngredient)
        CreateMap<RecipeIngredient, RecipeIngredientDto>()
            .ForMember(dest => dest.IngredientName,
                opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : "Невідомо"));

        // 5. Обрані рецепти (Favorites) з Flattening назви
        CreateMap<FavoriteRecipe, FavoriteRecipeDto>()
            .ForMember(dest => dest.RecipeTitle,
                opt => opt.MapFrom(src => src.Recipe != null ? src.Recipe.Title : "Рецепт видалено"))
            .ForMember(dest => dest.RecipeImageUrl,
                opt => opt.MapFrom(src => src.Recipe != null ? src.Recipe.ImageUrl : null));

        CreateMap<CreateFavoriteRecipeDto, FavoriteRecipe>();

    }
}