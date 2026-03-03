using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Extensions;

public static class EnumExtensions
{
    public static string ToTailwindZIndexClass(this ZIndex zIndex)
    {
        var value = zIndex switch
        {
            ZIndex.Zero => 0,
            ZIndex.Five => 5,
            ZIndex.Ten => 10,
            ZIndex.Twenty => 20,
            ZIndex.Thirty => 30,
            ZIndex.Forty => 40,
            ZIndex.Fifty => 50,
            ZIndex.Sixty => 60,
            ZIndex.Seventy => 70,
            ZIndex.Eighty => 80,
            ZIndex.Ninety => 90,
            ZIndex.OneHundred => 100,
            ZIndex.TwoHundred => 200,
            ZIndex.ThreeHundred => 300,
            ZIndex.FourHundred => 400,
            ZIndex.FiveHundred => 500,
            ZIndex.Thousand => 1000,
            _ => 0
        };

        return value.ToTailwindZIndexClass();
    }

    public static string ToTailwindZIndexClass(this int value)
    {
        return value <= 50
            ? $"z-{value}"
            : $"z-[{value}]";
    }

    public static string BringForward(this ZIndex zIndex)
    {
        var value = zIndex switch
        {
            ZIndex.Zero => 5,
            ZIndex.Five => 10,
            ZIndex.Ten => 20,
            ZIndex.Twenty => 30,
            ZIndex.Thirty => 40,
            ZIndex.Forty => 50,
            ZIndex.Fifty => 60,
            ZIndex.Sixty => 70,
            ZIndex.Seventy => 80,
            ZIndex.Eighty => 90,
            ZIndex.Ninety => 100,
            ZIndex.OneHundred => 200,
            ZIndex.TwoHundred => 300,
            ZIndex.ThreeHundred => 400,
            ZIndex.FourHundred => 500,
            ZIndex.FiveHundred => 1000,
            ZIndex.Thousand => 2000,
            _ => 3000
        };

        return value.ToTailwindZIndexClass();
    }
}