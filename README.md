# Kendo Grid latest features for C#/.NET (ASP.NET Core)

This repo summarizes the newest Kendo Grid features available in the latest
Telerik UI for ASP.NET Core feature release (What's New 2025 Q4) and provides
C#-centric snippets you can use as a starting point.

Sources:
- https://www.telerik.com/support/whats-new/aspnet-core-ui
- https://demos.telerik.com/aspnet-core/grid/ai-toolbar
- https://demos.telerik.com/aspnet-core/grid/stacked-display-mode

## New Grid features in the latest release

### 1) Prompt-Controlled DataGrid (AI Toolbar)
The ASP.NET Core Grid can now embed AI capabilities directly in the toolbar:
- AI Toolbar Tool for Data Operations: apply sorting, filtering, grouping, and
  other data operations using natural-language prompts.
- AI Toolbar Tool for Highlighting: highlight records matching conditions and
  reset highlights with a single action.
- AI Assistant column option: add a per-row assistant button that runs prompts
  against that row's data and inserts or copies the response.

### 2) Stacked Layout Mode for mobile-first grids
The Grid now supports Stacked Layout Mode, which renders each item as a vertical
card for small screens while preserving sorting, filtering, and editing.

## C# / Razor examples

### AI Toolbar Grid (HtmlHelper)
```cshtml
@using Kendo.Mvc.UI

@(Html.Kendo().Grid<FinanceTransaction>()
    .Name("grid")
    .Columns(columns =>
    {
        columns.Bound(p => p.CustomerName).Title("Customer Name").Locked(true).Width(200);
        columns.Bound(p => p.Amount).Title("Amount").Width(120).Format("{0:n}");
        columns.Bound(p => p.Status).Title("Status").Width(150);
        columns.Bound(p => p.TransDate).Title("Trans Date").Width(120).Format("{0:dd-MM-yy}");
    })
    .ToolBar(t =>
    {
        t.AIAssistant();
        t.Spacer();
        t.Custom().Name("resetChanges")
            .Text("Reset changes")
            .IconClass("k-icon k-i-arrow-rotate-ccw");
    })
    .AI(ai => ai
        .Service("https://demos.telerik.com/service/v2/ai/grid/smart-state")
        .AIAssistant(aiAsst => aiAsst
            .PromptSuggestions(new[]
            {
                "Sort by Amount descending and highlight only failed transactions",
                "Display 25 items per page",
                "Lock the Amount column",
                "Reorder Account Type to be first",
                "Export to PDF with file name 'report'"
            })
            .PromptTextArea(p => p.Rows(2).Resize(TextAreaResize.Auto).MaxRows(5))
        )
        .AIAssistantWindow(ws => ws.Width(558).Actions(a => a.Minimize().Close()))
    )
    .Sortable(s => s.SortMode(GridSortMode.MultipleColumn).AllowUnsort(true).ShowIndexes(true))
    .Filterable(f => f.Mode(GridFilterMode.Menu))
    .Groupable()
    .Pageable(p => p.PageSizes(new[] { 10 }))
    .Scrollable()
    .HtmlAttributes(new { style = "height:650px;" })
    .DataSource(ds => ds.Ajax()
        .Read(r => r.Action("Finance_Read", "Grid"))
        .Model(m => m.Id(x => x.Id))
        .PageSize(10)
    )
)
```

> Note: The AI service URL above is the public demo endpoint. Replace it with
> your own AI service in production.

### AI Toolbar data endpoint (Controller)
```csharp
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

public class GridController : Controller
{
    public ActionResult Finance_Read([DataSourceRequest] DataSourceRequest request)
    {
        var data = new[]
        {
            new FinanceTransaction
            {
                Id = 1,
                CustomerName = "Oliver Whitfield",
                Amount = 1250.75m,
                Status = "Completed",
                TransDate = new DateTime(2025, 11, 15)
            }
        };

        return Json(data.ToDataSourceResult(request));
    }
}

public class FinanceTransaction
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime TransDate { get; set; }
}
```

### Stacked Layout Mode (HtmlHelper)
```cshtml
@using Kendo.Mvc.UI

@(Html.Kendo().Grid<ProductViewModel>()
    .Name("grid")
    .DataLayoutMode(DataLayoutMode.Stacked)
    .StackedLayoutSettings(s => s.Cols(320, 120, 120))
    .Columns(columns =>
    {
        columns.Bound(p => p.ProductName).Title("Product");
        columns.Bound(p => p.UnitPrice).Title("Price");
        columns.Bound(p => p.UnitsInStock).Title("Units");
    })
    .ToolBar(toolbar =>
    {
        toolbar.Filter();
        toolbar.Sort();
        toolbar.Spacer();
        toolbar.ColumnChooser();
    })
    .AdaptiveMode(AdaptiveMode.Auto)
    .Pageable()
    .Sortable()
    .Filterable()
    .Scrollable()
    .DataSource(ds => ds.Ajax()
        .Read(r => r.Action("Products_Read", "Grid"))
        .PageSize(20)
    )
)
```
