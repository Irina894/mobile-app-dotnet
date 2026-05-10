using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WhatToCook.MAUI.Models;

using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.ViewModels;

public class WeekDay : INotifyPropertyChanged
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = string.Empty;
    public string DayNumber { get; set; } = string.Empty;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    private bool _hasMeals;
    public bool HasMeals
    {
        get => _hasMeals;
        set { _hasMeals = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}

public class DayEntryViewModel : INotifyPropertyChanged
{
    private readonly DateTime _viewDate;
    public CookingPlanEntry Entry { get; }

    public DayEntryViewModel(CookingPlanEntry entry, DateTime viewDate)
    {
        Entry = entry;
        _viewDate = viewDate;

        Entry.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsConsumedToday));
            OnPropertyChanged(nameof(ConsumedLabel));
            OnPropertyChanged(nameof(ConsumedTextColor));
        };
    }

    public bool IsToday => _viewDate.Date == DateTime.Today;
    public bool IsConsumedToday => Entry.IsConsumedOn(_viewDate);
    public string ConsumedLabel => IsConsumedToday ? "Eating today ✓" : "Skipped";
    public string ConsumedTextColor => IsConsumedToday ? "#16A34A" : "#9CA3AF";

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}

public class CalendarViewModel : INotifyPropertyChanged
{
    private readonly List<CookingPlanEntry> _allEntries = new();

    private int _householdSize = 1;
    public int HouseholdSize
    {
        get => _householdSize;
        set
        {
            if (value < 1) return;
            _householdSize = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HouseholdSizeText));
            Refresh();
        }
    }

    public string HouseholdSizeText => _householdSize == 1 ? "Just me" : $"{_householdSize} people";

    private DateTime _selectedDate = DateTime.Today;
    public string SelectedDateLabel => _selectedDate.ToString("dddd, MMMM d");

    private string _currentMonthYear = string.Empty;
    public string CurrentMonthYear
    {
        get => _currentMonthYear;
        set { _currentMonthYear = value; OnPropertyChanged(); }
    }

    public ObservableCollection<WeekDay> WeekDays { get; set; } = new();
    public ObservableCollection<DayEntryViewModel> DayView { get; set; } = new();
    public bool HasEntries => DayView.Count > 0;

    private bool _isModalVisible;
    public bool IsModalVisible
    {
        get => _isModalVisible;
        set { _isModalVisible = value; OnPropertyChanged(); }
    }

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set { _searchQuery = value; OnPropertyChanged(); ApplyModalFilter(); }
    }

    private int _activeTab;
    public int ActiveTab
    {
        get => _activeTab;
        set
        {
            _activeTab = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TabAllActive));
            OnPropertyChanged(nameof(TabFavActive));
            OnPropertyChanged(nameof(TabMyActive));
            ApplyModalFilter();
        }
    }

    public bool TabAllActive => _activeTab == 0;
    public bool TabFavActive => _activeTab == 1;
    public bool TabMyActive => _activeTab == 2;

    public ObservableCollection<RecipeModel> ModalRecipes { get; set; } = new();

    private int _newEntryPortions = 4;
    public int NewEntryPortions
    {
        get => _newEntryPortions;
        set
        {
            if (value < 1) return;
            _newEntryPortions = value;
            OnPropertyChanged();
        }
    }

    // Тимчасові тестові дані — пізніше замінити на завантаження з API
    private readonly List<RecipeModel> _allRecipes = new()
    {
        new RecipeModel { Id = 1, Title = "Borscht",         CookTimeMinutes = 60, Servings = 6, Description = "Traditional Ukrainian beet soup", Difficulty = "Medium", IsFavorite = false, IsMyRecipe = false },
        new RecipeModel { Id = 2, Title = "Omelette",        CookTimeMinutes = 10, Servings = 2, Description = "Fluffy egg omelette with cheese",  Difficulty = "Easy",   IsFavorite = true,  IsMyRecipe = false },
        new RecipeModel { Id = 3, Title = "Pasta Carbonara", CookTimeMinutes = 25, Servings = 4, Description = "Creamy pasta with bacon",          Difficulty = "Medium", IsFavorite = false, IsMyRecipe = true  },
        new RecipeModel { Id = 4, Title = "Greek Salad",     CookTimeMinutes = 5,  Servings = 2, Description = "Fresh salad with feta cheese",     Difficulty = "Easy",   IsFavorite = false, IsMyRecipe = false },
    };

    public ICommand AddRecipeCommand { get; }
    public ICommand RemoveEntryCommand { get; }
    public ICommand ToggleConsumedCommand { get; }
    public ICommand SelectDayCommand { get; }
    public ICommand PrevWeekCommand { get; }
    public ICommand NextWeekCommand { get; }
    public ICommand OpenModalCommand { get; }
    public ICommand CloseModalCommand { get; }
    public ICommand IncrementHouseholdCommand { get; }
    public ICommand DecrementHouseholdCommand { get; }
    public ICommand IncrementPortionsCommand { get; }
    public ICommand DecrementPortionsCommand { get; }
    public ICommand ViewRecipeCommand { get; }
    public ICommand TabAllCommand { get; }
    public ICommand TabFavCommand { get; }
    public ICommand TabMyCommand { get; }

    public CalendarViewModel()
    {
        SelectDayCommand = new Command<WeekDay>(OnSelectDay);
        PrevWeekCommand = new Command(OnPrevWeek);
        NextWeekCommand = new Command(OnNextWeek);
        OpenModalCommand = new Command(OnOpenModal);
        CloseModalCommand = new Command(OnCloseModal);
        IncrementHouseholdCommand = new Command(OnIncrementHousehold);
        DecrementHouseholdCommand = new Command(OnDecrementHousehold);
        IncrementPortionsCommand = new Command<CookingPlanEntry>(OnIncrementPortions);
        DecrementPortionsCommand = new Command<CookingPlanEntry>(OnDecrementPortions);
        ViewRecipeCommand = new Command<RecipeModel>(OnViewRecipe);
        TabAllCommand = new Command(OnTabAll);
        TabFavCommand = new Command(OnTabFav);
        TabMyCommand = new Command(OnTabMy);
        AddRecipeCommand = new Command<RecipeModel>(OnAddRecipe);
        RemoveEntryCommand = new Command<CookingPlanEntry>(OnRemoveEntry);
        ToggleConsumedCommand = new Command<DayEntryViewModel>(OnToggleConsumed);

        Refresh();
    }

    private void OnSelectDay(WeekDay? day)
    {
        if (day == null) return;
        _selectedDate = day.Date;
        Refresh();
    }

    private void OnPrevWeek() { _selectedDate = _selectedDate.AddDays(-7); Refresh(); }
    private void OnNextWeek() { _selectedDate = _selectedDate.AddDays(7); Refresh(); }
    private void OnOpenModal() => IsModalVisible = true;
    private void OnCloseModal()
    {
        IsModalVisible = false;
        SearchQuery = string.Empty;
        ActiveTab = 0;
    }

    private void OnIncrementHousehold() => HouseholdSize++;
    private void OnDecrementHousehold() { if (HouseholdSize > 1) HouseholdSize--; }

    private void OnIncrementPortions(CookingPlanEntry? entry)
    {
        if (entry == null) return;
        entry.TotalPortions++;
        Refresh();
    }

    private void OnDecrementPortions(CookingPlanEntry? entry)
    {
        if (entry == null || entry.TotalPortions <= 1) return;
        entry.TotalPortions--;
        Refresh();
    }

    private async void OnViewRecipe(RecipeModel? recipe)
    {
        if (recipe == null) return;
        await Application.Current!.MainPage!.DisplayAlert("Recipe Details",
            $"{recipe.Title}\n\n{recipe.Description}\n\nTime: {recipe.CookTimeMinutes} min\nServings: {recipe.Servings}\nDifficulty: {recipe.Difficulty}",
            "OK");
    }

    private void OnTabAll() => ActiveTab = 0;
    private void OnTabFav() => ActiveTab = 1;
    private void OnTabMy() => ActiveTab = 2;

    private void OnAddRecipe(RecipeModel? recipe)
    {
        if (recipe == null) return;

        bool alreadyExists = _allEntries.Any(e =>
            e.Recipe.Id == recipe.Id && e.CookedOn.Date == _selectedDate.Date);

        if (alreadyExists)
        {
            Application.Current!.MainPage!.DisplayAlert("Already Added",
                "This recipe is already planned for this day", "OK");
            return;
        }

        var entry = new CookingPlanEntry
        {
            Recipe = recipe,
            CookedOn = _selectedDate.Date,
            HouseholdSize = _householdSize,
            TotalPortions = NewEntryPortions
        };

        entry.RecalcDays();
        _allEntries.Add(entry);

        IsModalVisible = false;
        SearchQuery = string.Empty;
        ActiveTab = 0;
        NewEntryPortions = 4;
        Refresh();
    }

    private void OnRemoveEntry(CookingPlanEntry? entry)
    {
        if (entry == null) return;
        _allEntries.Remove(entry);
        Refresh();
    }

    private void OnToggleConsumed(DayEntryViewModel? vm)
    {
        if (vm?.Entry == null) return;
        vm.Entry.ToggleDay(_selectedDate);
        Refresh();
    }

    private void Refresh()
    {
        OnPropertyChanged(nameof(SelectedDateLabel));
        BuildWeek();
        BuildDayView();
    }

    private void BuildWeek()
    {
        CurrentMonthYear = _selectedDate.ToString("MMMM yyyy");

        var monday = _selectedDate.AddDays(-(int)_selectedDate.DayOfWeek + 1);
        if (_selectedDate.DayOfWeek == DayOfWeek.Sunday)
            monday = _selectedDate.AddDays(-6);

        WeekDays = new ObservableCollection<WeekDay>(
            Enumerable.Range(0, 7).Select(i =>
            {
                var date = monday.AddDays(i);
                return new WeekDay
                {
                    Date = date,
                    DayName = date.ToString("ddd"),
                    DayNumber = date.Day.ToString(),
                    IsSelected = date.Date == _selectedDate.Date,
                    HasMeals = GetEntries(date).Any(e => !e.IsFinished)
                };
            }));

        OnPropertyChanged(nameof(WeekDays));
    }

    private void BuildDayView()
    {
        DayView = new ObservableCollection<DayEntryViewModel>(
            GetEntries(_selectedDate).Select(e => new DayEntryViewModel(e, _selectedDate)));

        OnPropertyChanged(nameof(DayView));
        OnPropertyChanged(nameof(HasEntries));
    }

    private List<CookingPlanEntry> GetEntries(DateTime date) =>
        _allEntries.Where(e => e.SpansDate(date)).ToList();

    private void ApplyModalFilter()
    {
        var filtered = _allRecipes.AsEnumerable();

        filtered = _activeTab switch
        {
            1 => filtered.Where(r => r.IsFavorite),
            2 => filtered.Where(r => r.IsMyRecipe),
            _ => filtered
        };

        if (!string.IsNullOrWhiteSpace(SearchQuery))
            filtered = filtered.Where(r =>
                r.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        ModalRecipes = new ObservableCollection<RecipeModel>(filtered);
        OnPropertyChanged(nameof(ModalRecipes));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}