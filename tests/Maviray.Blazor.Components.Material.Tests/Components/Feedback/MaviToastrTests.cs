using Maviray.Blazor.Components.Material.Components.DataDisplay;
using Toastr = Maviray.Blazor.Components.Material.Components.DataDisplay.MaviToastr.MaviToastr;

namespace Maviray.Blazor.Components.Material.Tests.Components.Feedback;

public class MaviToastrTests : ComponentTestBase
{
    private readonly List<string> _closed = [];

    [Fact]
    public void AutoDismiss_RequestsClose_AfterDuration()
    {
        RenderToastr(TimeSpan.FromMilliseconds(20));

        WaitForAssertion(() => _closed.Should().Equal("t1"), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task AutoDismiss_DoesNotFire_AfterToastrIsDisposed()
    {
        RenderToastr(TimeSpan.FromMilliseconds(50));

        await DisposeComponentsAsync();
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);

        _closed.Should().BeEmpty();
    }

    private void RenderToastr(TimeSpan duration) =>
        Render<Toastr>(p => p
            .Add(x => x.Notifications, [new MaviToastNotification("t1", "Saved", "All good", Duration: duration)])
            .Add(x => x.OnClose, EventCallback.Factory.Create<string>(this, id => _closed.Add(id))));

    private static void WaitForAssertion(Action assertion, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;

        while (true)
        {
            try
            {
                assertion();
                return;
            }
            catch when (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(10);
            }
        }
    }
}
