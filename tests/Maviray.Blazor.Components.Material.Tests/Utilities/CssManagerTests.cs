using Maviray.Blazor.Components.Core.Utilities;

namespace Maviray.Blazor.Components.Material.Tests.Utilities;

public class CssManagerTests
{
    [Fact]
    public void Combine_SplitsTrimsAndDedupes()
    {
        CssManager.Combine("a  b", " b c\t", null, "", "a\nd").Should().Be("a b c d");
    }

    [Fact]
    public void Combine_WithNoClasses_ReturnsEmpty()
    {
        CssManager.Combine().Should().BeEmpty();
        CssManager.Combine(null, " ").Should().BeEmpty();
    }

    [Fact]
    public void Combine_AllocatesOnlyTheResult()
    {
        // Warm up so JIT tiering and pool setup are not counted.
        for (var i = 0; i < 100; i++)
        {
            CssManager.Combine("px-2 py-1", "text-sm", "px-2");
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        var result = CssManager.Combine("px-2 py-1", "text-sm", "px-2");
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        result.Should().Be("px-2 py-1 text-sm");
        allocated.Should().BeLessThan(120, "only the 17-character result string should be allocated");
    }
}
