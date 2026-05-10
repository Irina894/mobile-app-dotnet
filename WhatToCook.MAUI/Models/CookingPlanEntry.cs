using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecipeModel = WhatToCook.MAUI.Models.Recipe;

namespace WhatToCook.MAUI.Models;

public class DailyConsumption : INotifyPropertyChanged
{
    public DateTime Date { get; set; }

    private bool _consumed = true;
    public bool Consumed
    {
        get => _consumed;
        set { _consumed = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}

public class CookingPlanEntry : INotifyPropertyChanged
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public RecipeModel Recipe { get; set; } = new RecipeModel();

    public DateTime CookedOn { get; set; }

    private int _totalPortions = 4;
    public int TotalPortions
    {
        get => _totalPortions;
        set
        {
            if (value < 1) return;
            _totalPortions = value;
            OnPropertyChanged();
            RecalcDays();
        }
    }

    private int _householdSize = 1;
    public int HouseholdSize
    {
        get => _householdSize;
        set
        {
            if (value < 1) return;
            _householdSize = value;
            OnPropertyChanged();
            RecalcDays();
        }
    }

    public List<DailyConsumption> DailyPlan { get; private set; } = new();

    public int ConsumedPortions
    {
        get
        {
            int consumed = 0;
            foreach (var day in DailyPlan)
            {
                if (day.Date.Date <= DateTime.Today.Date && day.Consumed)
                    consumed += HouseholdSize;
            }
            return Math.Min(consumed, TotalPortions);
        }
    }

    public int RemainingPortions => Math.Max(0, TotalPortions - ConsumedPortions);
    public int TotalDays => (int)Math.Ceiling((double)TotalPortions / HouseholdSize);

    public int DaysLeft
    {
        get
        {
            if (RemainingPortions == 0) return 0;
            int futureDays = 0;
            int remaining = RemainingPortions;
            foreach (var day in DailyPlan.OrderBy(d => d.Date))
            {
                if (day.Date.Date >= DateTime.Today.Date && remaining > 0)
                {
                    futureDays++;
                    remaining -= HouseholdSize;
                }
            }
            return futureDays;
        }
    }

    public DateTime LastDay => CookedOn.Date.AddDays(TotalDays - 1);
    public bool IsLastDay => DaysLeft == 1 && RemainingPortions > 0 && RemainingPortions <= HouseholdSize;
    public bool IsFinished => RemainingPortions == 0;

    public string StatusText
    {
        get
        {
            if (IsFinished) return "Finished";
            if (IsLastDay) return "Last day! ⚠️";
            if (TotalDays == 1) return "Single meal";
            return DaysLeft == 1 ? "1 day left" : $"{DaysLeft} days left";
        }
    }

    public string StatusColor
    {
        get
        {
            if (IsFinished) return "#9CA3AF";
            if (IsLastDay) return "#EF4444";
            if (TotalDays == 1) return "#8B5CF6";
            if (DaysLeft <= 2) return "#F59E0B";
            return "#16A34A";
        }
    }

    public string StatusBackground
    {
        get
        {
            if (IsFinished) return "#F3F4F6";
            if (IsLastDay) return "#FEE2E2";
            if (TotalDays == 1) return "#EDE9FE";
            if (DaysLeft <= 2) return "#FEF3C7";
            return "#DCFCE7";
        }
    }

    public string PortionsText
    {
        get
        {
            if (IsFinished) return "All portions eaten";
            if (TotalDays == 1) return $"{RemainingPortions} of {TotalPortions} portion";
            return $"{RemainingPortions} of {TotalPortions} portions left";
        }
    }

    public void RecalcDays()
    {
        var existing = DailyPlan.ToDictionary(d => d.Date.Date);
        var newPlan = new List<DailyConsumption>();

        for (int i = 0; i < TotalDays; i++)
        {
            var date = CookedOn.Date.AddDays(i);
            if (existing.TryGetValue(date, out var prev))
                newPlan.Add(prev);
            else
                newPlan.Add(new DailyConsumption { Date = date, Consumed = true });
        }

        DailyPlan = newPlan;
        NotifyAll();
    }

    public void ToggleDay(DateTime date)
    {
        var day = DailyPlan.FirstOrDefault(d => d.Date.Date == date.Date);
        if (day == null) return;
        day.Consumed = !day.Consumed;
        NotifyAll();
    }

    public bool SpansDate(DateTime date) =>
        DailyPlan.Any(d => d.Date.Date == date.Date);

    public bool IsConsumedOn(DateTime date) =>
        DailyPlan.FirstOrDefault(d => d.Date.Date == date.Date)?.Consumed ?? false;

    public void NotifyAll()
    {
        OnPropertyChanged(nameof(ConsumedPortions));
        OnPropertyChanged(nameof(RemainingPortions));
        OnPropertyChanged(nameof(DaysLeft));
        OnPropertyChanged(nameof(TotalDays));
        OnPropertyChanged(nameof(LastDay));
        OnPropertyChanged(nameof(IsLastDay));
        OnPropertyChanged(nameof(IsFinished));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(StatusBackground));
        OnPropertyChanged(nameof(PortionsText));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string n = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}