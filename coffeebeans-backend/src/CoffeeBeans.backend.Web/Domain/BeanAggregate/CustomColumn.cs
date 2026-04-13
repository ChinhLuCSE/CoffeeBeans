namespace CoffeeBeans.backend.Web.Domain.BeanAggregate;

public enum CustomColumnType { Integer, Double, String }

public sealed class CustomColumn : EntityBase<CustomColumn, CustomColumnId>
{
    public string ColumnName { get; private set; } = default!;
    public CustomColumnType DataType { get; private set; }

    private readonly List<BeanCustomValue> _customValues = new();
    public IReadOnlyList<BeanCustomValue> CustomValues => _customValues.AsReadOnly();
    
    private CustomColumn() { }

    public CustomColumn(string columnName, CustomColumnType dataType)
        : this(CustomColumnId.From(Guid.NewGuid()), columnName, dataType)
    {
    }

    public CustomColumn(CustomColumnId customColumnId, string columnName, CustomColumnType dataType)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            throw new ArgumentException("Column name is required.", nameof(columnName));

        Id = customColumnId;
        ColumnName = columnName;
        DataType = dataType;
    }
}
