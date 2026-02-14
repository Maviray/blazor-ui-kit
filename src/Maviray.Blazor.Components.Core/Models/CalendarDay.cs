namespace Maviray.Blazor.Components.Core.Models;

public class CalendarDay
{
    public DateTime Date { get; set; }
    public bool IsInCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool IsSelected { get; set; }
}