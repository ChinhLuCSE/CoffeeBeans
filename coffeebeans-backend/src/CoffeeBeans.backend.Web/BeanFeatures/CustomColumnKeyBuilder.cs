using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.BeanFeatures;

internal static class CustomColumnKeyBuilder
{
  public static IReadOnlyList<CustomColumnKeyDefinition> Build(IEnumerable<CustomColumn> customColumns)
  {
    var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var definitions = new List<CustomColumnKeyDefinition>();

    foreach (var column in customColumns.OrderBy(static column => column.ColumnName, StringComparer.OrdinalIgnoreCase))
    {
      var baseKey = $"custom_{Slugify(column.ColumnName)}";
      var key = baseKey;
      var suffix = 2;

      while (!usedKeys.Add(key))
      {
        key = $"{baseKey}_{suffix}";
        suffix++;
      }

      definitions.Add(new CustomColumnKeyDefinition(column.Id.Value, column.ColumnName, key, column.DataType));
    }

    return definitions;
  }

  private static string Slugify(string value)
  {
    var buffer = new List<char>(value.Length);
    var previousWasUnderscore = false;

    foreach (var character in value.Trim().ToLowerInvariant())
    {
      if (char.IsLetterOrDigit(character))
      {
        buffer.Add(character);
        previousWasUnderscore = false;
      }
      else if (!previousWasUnderscore)
      {
        buffer.Add('_');
        previousWasUnderscore = true;
      }
    }

    var slug = new string(buffer.ToArray()).Trim('_');
    return string.IsNullOrWhiteSpace(slug) ? "column" : slug;
  }
}

internal sealed record CustomColumnKeyDefinition(
  Guid Id,
  string Name,
  string Key,
  CustomColumnType DataType);
