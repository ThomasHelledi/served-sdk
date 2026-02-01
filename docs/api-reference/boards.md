# Boards API

Spreadsheet-like boards with sheets, columns, rows, and cells.

Access via: `client.BoardModule`

---

## Boards

### GetBoardsAsync

Get all boards for the tenant.

```csharp
var boards = await client.BoardModule.GetBoardsAsync();

foreach (var board in boards)
{
    Console.WriteLine($"{board.Name} ({board.SheetCount} sheets)");
}
```

---

### GetBoardAsync

Get a board by ID.

```csharp
var board = await client.BoardModule.GetBoardAsync(123);

Console.WriteLine($"Board: {board.Name}");
Console.WriteLine($"Description: {board.Description}");
Console.WriteLine($"Sheets: {board.SheetCount}");
```

---

### GetBoardKeysAsync

Get board IDs for the tenant.

```csharp
var boardIds = await client.BoardModule.GetBoardKeysAsync();
// Returns: [1, 2, 3, ...]
```

---

### CreateBoardAsync

Create a new board.

```csharp
var board = await client.BoardModule.CreateBoardAsync(
    new BoardViewModel
    {
        Name = "Product Roadmap",
        Description = "Q1-Q4 2026 Planning"
    }
);
```

---

### UpdateBoardAsync

Update board settings.

```csharp
var board = await client.BoardModule.GetBoardAsync(123);
board.Name = "Product Roadmap 2026";
board.Description = "Updated planning board";

await client.BoardModule.UpdateBoardAsync(board);
```

---

### DeleteBoardAsync

Delete a board.

```csharp
await client.BoardModule.DeleteBoardAsync(123);
```

> **Warning:** This deletes all sheets in the board.

---

## Sheets

### GetSheetsAsync

Get all sheets for the tenant.

```csharp
var sheets = await client.BoardModule.GetSheetsAsync();

foreach (var sheet in sheets)
{
    Console.WriteLine($"{sheet.Name} - {sheet.ColumnCount} columns, {sheet.RowCount} rows");
}
```

---

### GetSheetAsync

Get a sheet by ID.

```csharp
var sheet = await client.BoardModule.GetSheetAsync(456);

Console.WriteLine($"Sheet: {sheet.Name}");
Console.WriteLine($"Board: {sheet.BoardName}");
```

---

### GetSheetByClaimIdAsync

Get a sheet by its claim ID (GUID).

```csharp
var sheet = await client.BoardModule.GetSheetByClaimIdAsync(
    Guid.Parse("550e8400-e29b-41d4-a716-446655440000")
);
```

---

### CreateSheetAsync

Create a new sheet.

```csharp
var sheet = await client.BoardModule.CreateSheetAsync(
    new CreateSheetRequest
    {
        BoardId = 123,
        Name = "Sprint Backlog",
        Description = "Current sprint items"
    }
);
```

---

### UpdateSheetAsync

Update sheet settings.

```csharp
await client.BoardModule.UpdateSheetAsync(
    new UpdateSheetRequest
    {
        Id = 456,
        Name = "Sprint 42 Backlog",
        Description = "Sprint 42 (Feb 1-14)"
    }
);
```

---

### DeleteSheetAsync

Delete a sheet.

```csharp
await client.BoardModule.DeleteSheetAsync(456);
```

---

## Columns

### GetColumnsAsync

Get columns for a sheet.

```csharp
var columns = await client.BoardModule.GetColumnsAsync(sheetId: 456);

foreach (var col in columns.OrderBy(c => c.Order))
{
    Console.WriteLine($"{col.Name} ({col.Type})");
}
```

---

### AddColumnAsync

Add a column to a sheet.

```csharp
var column = await client.BoardModule.AddColumnAsync(
    sheetId: 456,
    new CreateSheetColumnRequest
    {
        Name = "Status",
        Type = "select",
        Options = new List<string> { "Todo", "In Progress", "Done" }
    }
);
```

**Column Types:**
- `text` - Plain text
- `number` - Numeric value
- `date` - Date picker
- `select` - Single select dropdown
- `multiselect` - Multi-select tags
- `checkbox` - Boolean checkbox
- `person` - User reference
- `link` - URL link
- `formula` - Calculated value

---

### DeleteColumnAsync

Delete a column.

```csharp
await client.BoardModule.DeleteColumnAsync(columnId: 789);
```

---

### ReorderColumnsAsync

Reorder columns in a sheet.

```csharp
await client.BoardModule.ReorderColumnsAsync(
    sheetId: 456,
    columnIds: new[] { 3, 1, 2, 4 }  // New order
);
```

---

## Rows

### GetRowsAsync

Get rows for a sheet with pagination.

```csharp
// First 100 rows
var rows = await client.BoardModule.GetRowsAsync(sheetId: 456);

// Paginated
var page2 = await client.BoardModule.GetRowsAsync(
    sheetId: 456,
    skip: 100,
    take: 100
);
```

---

### AddRowAsync

Add a row to a sheet.

```csharp
var row = await client.BoardModule.AddRowAsync(
    new CreateSheetRowRequest
    {
        SheetId = 456,
        Values = new Dictionary<int, object>
        {
            { 1, "New Task" },         // Column 1: Name
            { 2, "In Progress" },      // Column 2: Status
            { 3, DateTime.Today }      // Column 3: Due Date
        }
    }
);
```

---

### DeleteRowAsync

Delete a row.

```csharp
await client.BoardModule.DeleteRowAsync(rowId: 1001);

// Or with sheet context
await client.BoardModule.DeleteRowAsync(rowId: 1001, sheetId: 456);
```

---

### ReorderRowsAsync

Reorder rows in a sheet.

```csharp
await client.BoardModule.ReorderRowsAsync(
    sheetId: 456,
    rowIds: new[] { 1003, 1001, 1002 }  // New order
);
```

---

## Cells

### GetCellAsync

Get a specific cell value.

```csharp
var cell = await client.BoardModule.GetCellAsync(
    rowId: 1001,
    columnId: 789
);

Console.WriteLine($"Value: {cell.Value}");
Console.WriteLine($"Display: {cell.DisplayValue}");
```

---

### GetCellsForRowAsync

Get all cells in a row.

```csharp
var cells = await client.BoardModule.GetCellsForRowAsync(rowId: 1001);

foreach (var cell in cells)
{
    Console.WriteLine($"{cell.ColumnName}: {cell.DisplayValue}");
}
```

---

### GetCellsForColumnAsync

Get all cells in a column.

```csharp
var cells = await client.BoardModule.GetCellsForColumnAsync(columnId: 789);

var values = cells.Select(c => c.Value).Distinct();
Console.WriteLine($"Unique values: {string.Join(", ", values)}");
```

---

### SetCellValueAsync

Set a cell value.

```csharp
// Text value
await client.BoardModule.SetCellValueAsync(
    rowId: 1001,
    columnId: 789,
    value: "Updated text"
);

// Number value
await client.BoardModule.SetCellValueAsync(
    rowId: 1001,
    columnId: 790,
    value: 42.5
);

// Date value
await client.BoardModule.SetCellValueAsync(
    rowId: 1001,
    columnId: 791,
    value: DateTime.Today
);

// Clear value
await client.BoardModule.SetCellValueAsync(
    rowId: 1001,
    columnId: 789,
    value: null
);
```

---

## Views

### GetViewsAsync

Get views for a sheet.

```csharp
var views = await client.BoardModule.GetViewsAsync(sheetId: 456);

foreach (var view in views)
{
    Console.WriteLine($"{view.Name} ({view.Type})");
}
```

---

### AddViewAsync

Create a new view.

```csharp
var view = await client.BoardModule.AddViewAsync(
    new CreateSheetViewRequest
    {
        SheetId = 456,
        Name = "In Progress Items",
        Type = "grid",
        Filters = new List<ViewFilter>
        {
            new() { ColumnId = 2, Operator = "equals", Value = "In Progress" }
        },
        SortBy = new List<ViewSort>
        {
            new() { ColumnId = 3, Direction = "asc" }  // Sort by due date
        }
    }
);
```

**View Types:**
- `grid` - Table view (default)
- `kanban` - Kanban board
- `calendar` - Calendar view
- `gallery` - Card gallery
- `timeline` - Timeline/Gantt

---

### DeleteViewAsync

Delete a view.

```csharp
await client.BoardModule.DeleteViewAsync(viewId: 999);
```

---

## Models

### BoardViewModel

```csharp
public class BoardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int SheetCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### SheetViewModel

```csharp
public class SheetViewModel
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public string BoardName { get; set; }
    public Guid ClaimId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ColumnCount { get; set; }
    public int RowCount { get; set; }
}
```

### SheetColumnViewModel

```csharp
public class SheetColumnViewModel
{
    public int Id { get; set; }
    public int SheetId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }
    public int Width { get; set; }
    public List<string> Options { get; set; }
    public string Formula { get; set; }
}
```

### SheetRowViewModel

```csharp
public class SheetRowViewModel
{
    public int Id { get; set; }
    public int SheetId { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<int, SheetCellViewModel> Cells { get; set; }
}
```

### SheetCellViewModel

```csharp
public class SheetCellViewModel
{
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public string ColumnName { get; set; }
    public object Value { get; set; }
    public string DisplayValue { get; set; }
}
```

---

## Common Patterns

### Spreadsheet Data Export

```csharp
public async Task<List<Dictionary<string, object>>> ExportSheetAsync(int sheetId)
{
    var columns = await _client.BoardModule.GetColumnsAsync(sheetId);
    var rows = await _client.BoardModule.GetRowsAsync(sheetId, take: 10000);

    var result = new List<Dictionary<string, object>>();

    foreach (var row in rows)
    {
        var rowData = new Dictionary<string, object>();
        foreach (var col in columns)
        {
            if (row.Cells.TryGetValue(col.Id, out var cell))
            {
                rowData[col.Name] = cell.Value;
            }
        }
        result.Add(rowData);
    }

    return result;
}
```

### Bulk Row Import

```csharp
public async Task ImportRowsAsync(int sheetId, List<Dictionary<string, object>> data)
{
    var columns = await _client.BoardModule.GetColumnsAsync(sheetId);
    var columnMap = columns.ToDictionary(c => c.Name, c => c.Id);

    foreach (var rowData in data)
    {
        var values = new Dictionary<int, object>();
        foreach (var kvp in rowData)
        {
            if (columnMap.TryGetValue(kvp.Key, out var columnId))
            {
                values[columnId] = kvp.Value;
            }
        }

        await _client.BoardModule.AddRowAsync(
            new CreateSheetRowRequest
            {
                SheetId = sheetId,
                Values = values
            }
        );
    }
}
```

### Kanban Board Summary

```csharp
public async Task<KanbanSummary> GetKanbanSummaryAsync(int sheetId, int statusColumnId)
{
    var cells = await _client.BoardModule.GetCellsForColumnAsync(statusColumnId);

    return new KanbanSummary
    {
        TotalItems = cells.Count,
        ByStatus = cells
            .GroupBy(c => c.DisplayValue ?? "No Status")
            .ToDictionary(
                g => g.Key,
                g => g.Count()
            )
    };
}
```

### Dynamic Table Builder

```csharp
public async Task<int> CreateTableAsync(int boardId, string name, List<ColumnDefinition> columns)
{
    // Create sheet
    var sheet = await _client.BoardModule.CreateSheetAsync(
        new CreateSheetRequest
        {
            BoardId = boardId,
            Name = name
        }
    );

    // Add columns
    foreach (var col in columns)
    {
        await _client.BoardModule.AddColumnAsync(
            sheet.Id,
            new CreateSheetColumnRequest
            {
                Name = col.Name,
                Type = col.Type,
                Options = col.Options
            }
        );
    }

    return sheet.Id;
}
```
