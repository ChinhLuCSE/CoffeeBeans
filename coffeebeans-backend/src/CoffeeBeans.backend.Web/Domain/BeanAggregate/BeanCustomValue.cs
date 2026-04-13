namespace CoffeeBeans.backend.Web.Domain.BeanAggregate;

// public sealed class BeanCustomValue
// {
//     public int Id { get; set; }
//     public int BeanId { get; set; }
//     public Bean Bean { get; set; } = default!;
//     public int CustomColumnId { get; set; }
//     public CustomColumn CustomColumn { get; set; } = default!;
//     public int? IntValue { get; set; }
//     public double? FloatValue { get; set; }
//     public string? StringValue { get; set; }
// }

public class BeanCustomValue
{
    // Private constructor for EF Core
    private BeanCustomValue() { }

    public BeanCustomValue(BeanId beanId, CustomColumnId customColumnId, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", nameof(value));

        BeanId = beanId;
        CustomColumnId = customColumnId;
        Value = value;
    }

    public BeanId BeanId { get; private set; }
    public CustomColumnId CustomColumnId { get; private set; }
    public string Value { get; private set; } = default!;

    public Bean Bean { get; private set; } = default!;
    public CustomColumn CustomColumn { get; private set; } = default!;
}
