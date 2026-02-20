using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Models.Toastr;
using Maviray.Blazor.Components.Core.Options;
using Maviray.Blazor.Components.Core.Services;
using Maviray.Blazor.Components.Material.Components.DataDisplay;
using Microsoft.Extensions.DependencyInjection;

namespace Maviray.Blazor.Components.Material.Tests.Components.DataDisplay;

public class MaviToastrTests : ComponentTestBase
{
    private IMaviToastrService GetService() => Services.GetRequiredService<IMaviToastrService>();

    // ─── Stacking order ──────────────────────────────────────────────────────

    [Fact]
    public void Show_MultipleToasts_NewestFirst()
    {
        var svc = GetService();

        svc.Show(new ToastItem { Message = "First", Duration = 0 });
        svc.Show(new ToastItem { Message = "Second", Duration = 0 });
        svc.Show(new ToastItem { Message = "Third", Duration = 0 });

        var toasts = svc.ActiveToasts;
        toasts[0].Message.Should().Be("Third");
        toasts[1].Message.Should().Be("Second");
        toasts[2].Message.Should().Be("First");
    }

    // ─── Sticky toasts ────────────────────────────────────────────────────────

    [Fact]
    public void StickyToast_IsNotAutoScheduledForDismissal()
    {
        var svc = GetService();

        var item = new ToastItem { Message = "Stay", Duration = 0 };
        svc.Show(item);

        item.IsSticky.Should().BeTrue();
        svc.ActiveToasts.Should().ContainSingle(t => t.Id == item.Id);
    }

    [Fact]
    public void Dismiss_RemovesToast()
    {
        var svc = GetService();

        var item = new ToastItem { Message = "Toast", Duration = 0 };
        svc.Show(item);

        svc.Dismiss(item.Id);

        svc.ActiveToasts.Should().NotContain(t => t.Id == item.Id);
    }

    [Fact]
    public void DismissAll_ClearsAllToasts()
    {
        var svc = GetService();

        svc.Show(new ToastItem { Message = "A", Duration = 0 });
        svc.Show(new ToastItem { Message = "B", Duration = 0 });

        svc.DismissAll();

        svc.ActiveToasts.Should().BeEmpty();
    }

    // ─── Auto-dismiss ─────────────────────────────────────────────────────────

    [Fact]
    public async Task TimedToast_IsAutoDismissed()
    {
        var svc = GetService();

        var item = new ToastItem { Message = "Timed", Duration = 50 };
        svc.Show(item);

        await Task.Delay(200);

        svc.ActiveToasts.Should().NotContain(t => t.Id == item.Id);
    }

    // ─── MaxVisibleCount ──────────────────────────────────────────────────────

    [Fact]
    public void Show_ExceedsMaxVisibleCount_OldestDropped()
    {
        Services.AddSingleton(new MaviToastrOptions { MaxVisibleCount = 2 });
        Services.AddSingleton<IMaviToastrService, MaviToastrService>();
        var svc = Services.GetRequiredService<IMaviToastrService>();

        svc.Show(new ToastItem { Message = "A", Duration = 0 });
        svc.Show(new ToastItem { Message = "B", Duration = 0 });
        svc.Show(new ToastItem { Message = "C", Duration = 0 });

        svc.ActiveToasts.Should().HaveCount(2);
        svc.ActiveToasts.Should().NotContain(t => t.Message == "A");
    }

    // ─── Position class mapping ───────────────────────────────────────────────

    [Theory]
    [InlineData(ToastrPosition.CenterTop, "left-1/2")]
    [InlineData(ToastrPosition.TopLeft, "top-4 left-4")]
    [InlineData(ToastrPosition.TopRight, "top-4 right-4")]
    [InlineData(ToastrPosition.BottomLeft, "bottom-4 left-4")]
    [InlineData(ToastrPosition.BottomRight, "bottom-4 right-4")]
    [InlineData(ToastrPosition.CenterBottom, "bottom-4")]
    public void MaviToastrHost_RendersContainerWithCorrectPositionClass(
        ToastrPosition position,
        string expectedClassFragment)
    {
        var svc = GetService();
        svc.Show(new ToastItem { Message = "Test", Position = position, Duration = 0 });

        var cut = Render<MaviToastrHost>();

        cut.Markup.Should().Contain(expectedClassFragment);
    }

    // ─── MaviToastrHost renders toasts ───────────────────────────────────────

    [Fact]
    public void MaviToastrHost_RendersActiveToasts()
    {
        var svc = GetService();
        svc.Show(new ToastItem { Message = "Hello World", Duration = 0 });

        var cut = Render<MaviToastrHost>();

        cut.Markup.Should().Contain("Hello World");
    }

    [Fact]
    public void MaviToastrHost_UpdatesWhenToastAdded()
    {
        var svc = GetService();
        var cut = Render<MaviToastrHost>();

        cut.Markup.Should().NotContain("Late Toast");

        svc.Show(new ToastItem { Message = "Late Toast", Duration = 0 });
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Late Toast"));
    }

    // ─── MaviToastrItem renders correctly ────────────────────────────────────

    [Fact]
    public void MaviToastrItem_ShowsProgressBarForTimedToast()
    {
        var toast = new ToastItem { Message = "Timed", Duration = 5000 };
        var cut = Render<MaviToastrItem>(p =>
            p.Add(x => x.Toast, toast));

        // Progress bar element is rendered with the animation style
        cut.Markup.Should().Contain("animation: mavi-toast-shrink");
    }

    [Fact]
    public void MaviToastrItem_HidesProgressBarForStickyToast()
    {
        var toast = new ToastItem { Message = "Sticky", Duration = 0 };
        var cut = Render<MaviToastrItem>(p =>
            p.Add(x => x.Toast, toast));

        // Progress bar element is not rendered for sticky toasts (keyframe in <style> is always present)
        cut.Markup.Should().NotContain("animation: mavi-toast-shrink");
    }

    [Fact]
    public void MaviToastrItem_RendersTitle()
    {
        var toast = new ToastItem { Title = "My Title", Message = "My Message", Duration = 0 };
        var cut = Render<MaviToastrItem>(p =>
            p.Add(x => x.Toast, toast));

        cut.Markup.Should().Contain("My Title");
        cut.Markup.Should().Contain("My Message");
    }
}
