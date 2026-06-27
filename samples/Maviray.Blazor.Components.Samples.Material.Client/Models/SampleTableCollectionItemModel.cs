using System.ComponentModel.DataAnnotations;
using Maviray.Blazor.Components.Core.Attributes;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Models.Tables;
using Maviray.Blazor.Components.Samples.Material.Client.Enums;

namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleTableCollectionItemModel : ITableDataItem
{
    [Display(Name = "String")]
    [TableColumn(1, true)]
    public string? SampleString { get; set; }

    [Display(Name = "Enum")]
    [TableColumn(2)]
    public SampleEnum SampleEnum { get; set; }

    [Display(Name = "Boolean")]
    [TableColumn(3, "Positive", "Negative")]
    public bool? SampleBoolean { get; set; }

    [Display(Name = "Integer")]
    [TableColumn(4, HorizontalPosition.Left)]
    public int? SampleInteger { get; set; }

    [Display(Name = "Decimal")]
    [TableColumn(5, HorizontalPosition.Right)]
    public decimal? SampleDecimal { get; set; }

    [Display(Name = "Double")]
    [TableColumn(6)]
    public double? SampleDouble { get; set; }

    [Display(Name = "DateTime")]
    [TableColumn(7, "dd.MM.yyyy")]
    public DateTime? SampleDateTime { get; set; }

    public int Id { get; set; }
    public string? Guid { get; set; }

    public IEnumerable<MaviTableRowContextMenuItem> ContextMenu { get; set; } = [];
}