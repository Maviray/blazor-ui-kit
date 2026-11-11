using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Material.Components.Tabs;
using Maviray.Blazor.Components.Material.Tests.TestUtilities;

namespace Maviray.Blazor.Components.Material.Tests.Components.Tabs;

public class MaviTabNavBarTests : ComponentTestBase
{
    private static RenderFragment SingleTab => builder =>
    {
        builder.OpenComponent<MaviTab>(0);
        builder.AddAttribute(1, nameof(MaviTab.Id), "tab-a");
        builder.AddAttribute(2, nameof(MaviTab.Title), "A");
        builder.CloseComponent();
    };

    [Fact]
    public void TabClick_RaisesOnTabClick_Once_AfterParentReRenders()
    {
        var clicks = 0;
        var cut = Render<MaviTabNavBar>(p => p
            .Add(x => x.OnTabClick, EventCallback.Factory.Create<TabClickEventArgs>(this, _ => clicks++))
            .Add(x => x.ChildContent, SingleTab));

        // Any parent re-render re-applies the nav bar's parameters.
        cut.Render(p => p.Add(x => x.ElementSize, ElementSize.Small));
        cut.Render(p => p.Add(x => x.ElementSize, ElementSize.Large));

        cut.Find("#tab-a").Click();

        clicks.Should().Be(1);
    }

    [Fact]
    public void ParentReRenders_DoNotStackTabClickSubscriptions()
    {
        var cut = Render<MaviTabNavBar>(p => p.Add(x => x.ChildContent, SingleTab));

        cut.Render(p => p.Add(x => x.ElementSize, ElementSize.Small));
        cut.Render(p => p.Add(x => x.ElementSize, ElementSize.Large));

        // One handler for the nav bar and one for its single tab.
        EventSubscriptions.Count(cut.Instance.Context, nameof(MaviTabContext.OnTabClicked)).Should().Be(2);
    }
}
