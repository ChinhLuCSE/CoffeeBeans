using CoffeeBeans.backend.Web.BeanFeatures;
using CoffeeBeans.backend.Web.BeanFeatures.List;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeans.backend.Web.Infrastructure.Data.Queries;

public class ListBeansQueryService : IListBeansQueryService
{
  private readonly AppDbContext _db;

  public ListBeansQueryService(AppDbContext db)
  {
    _db = db;
  }

  public async Task<BeanListDto> ListAsync(ListBeansQuery query, CancellationToken cancellationToken)
  {
    var customColumns = await _db.CustomColumns
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    var beans = await _db.Beans
      .AsNoTracking()
      .Include(static bean => bean.CustomValues)
      .ThenInclude(static value => value.CustomColumn)
      .ToListAsync(cancellationToken);

    var customColumnDefinitions = BuildCustomColumnDefinitions(customColumns);
    var beanRows = beans
      .Select(bean => CreateBeanRow(bean, customColumnDefinitions))
      .ToList();

    var filteredRows = ApplyFilters(beanRows, query.Filters);
    var sortedRows = ApplySorting(filteredRows, query.SortBy, query.SortDir);

    var totalCount = sortedRows.Count;
    var page = Math.Max(query.Page ?? 1, 1);
    var perPage = Math.Max(query.PerPage ?? Constants.DEFAULT_PAGE_SIZE, 1);
    var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)perPage);

    var pagedItems = sortedRows
      .Skip((page - 1) * perPage)
      .Take(perPage)
      .Select(static row => row.Dto)
      .ToList();

    var responseColumns = customColumnDefinitions
      .Select(static column => new CustomColumnDto(column.Id, column.Name, column.Key, ToApiDataType(column.DataType)))
      .ToList();

    return new BeanListDto(pagedItems, responseColumns, page, perPage, totalCount, totalPages);
  }

  private static List<BeanRow> ApplyFilters(IEnumerable<BeanRow> rows, IReadOnlyDictionary<string, string>? filters)
  {
    if (filters is null || filters.Count == 0)
    {
      return rows.ToList();
    }

    return rows.Where(row => filters.All(filter => MatchesFilter(row, filter.Key, filter.Value))).ToList();
  }

  private static List<BeanRow> ApplySorting(IEnumerable<BeanRow> rows, string? sortBy, string? sortDir)
  {
    var normalizedSortBy = NormalizeKey(sortBy);
    var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

    if (string.IsNullOrWhiteSpace(normalizedSortBy))
    {
      return rows.OrderBy(static row => row.Dto.BeanId.Value).ToList();
    }

    var comparer = Comparer<BeanRow>.Create((left, right) =>
    {
      var comparison = CompareSortValues(GetFieldValue(left, normalizedSortBy), GetFieldValue(right, normalizedSortBy));
      if (comparison != 0)
      {
        return descending ? -comparison : comparison;
      }

      return left.Dto.BeanId.Value.CompareTo(right.Dto.BeanId.Value);
    });

    return rows.OrderBy(static row => row, comparer).ToList();
  }

  private static bool MatchesFilter(BeanRow row, string filterKey, string filterValue)
  {
    if (string.IsNullOrWhiteSpace(filterValue))
    {
      return true;
    }

    var fieldValue = GetFieldValue(row, filterKey);
    if (fieldValue is null)
    {
      return false;
    }

    return fieldValue switch
    {
      string text => text.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
      int intValue when int.TryParse(filterValue, out var parsedInt) => intValue == parsedInt,
      double doubleValue when double.TryParse(filterValue, out var parsedDouble) => Math.Abs(doubleValue - parsedDouble) < 0.000001d,
      Guid guidValue when Guid.TryParse(filterValue, out var parsedGuid) => guidValue == parsedGuid,
      _ => string.Equals(fieldValue.ToString(), filterValue, StringComparison.OrdinalIgnoreCase)
    };
  }

  private static object? GetFieldValue(BeanRow row, string? fieldName)
  {
    var normalizedField = NormalizeKey(fieldName);

    return normalizedField switch
    {
      "id" => row.Dto.BeanId.Value,
      "tasting_profile" => row.Dto.TastingProfile,
      "bag_weight_g" => row.Dto.BagWeightG,
      "roast_index" => row.Dto.RoastIndex,
      "num_farms" => row.Dto.NumFarms,
      "num_acidity_notes" => row.Dto.NumAcidityNotes,
      "num_sweetness_notes" => row.Dto.NumSweetnessNotes,
      "x" => row.Dto.X,
      "y" => row.Dto.Y,
      _ when normalizedField is not null && row.Dto.CustomFields.TryGetValue(normalizedField, out var customValue) => customValue,
      _ => null
    };
  }

  private static int CompareSortValues(object? left, object? right)
  {
    if (ReferenceEquals(left, right))
    {
      return 0;
    }

    if (left is null)
    {
      return 1;
    }

    if (right is null)
    {
      return -1;
    }

    return (left, right) switch
    {
      (string leftText, string rightText) => StringComparer.OrdinalIgnoreCase.Compare(leftText, rightText),
      (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
      (double leftDouble, double rightDouble) => leftDouble.CompareTo(rightDouble),
      (Guid leftGuid, Guid rightGuid) => leftGuid.CompareTo(rightGuid),
      _ => StringComparer.OrdinalIgnoreCase.Compare(left.ToString(), right.ToString())
    };
  }

  private static BeanRow CreateBeanRow(Bean bean, IReadOnlyList<CustomColumnDefinition> customColumns)
  {
    var customFields = customColumns.ToDictionary(static column => column.Key,
                                                  static _ => (object?)null,
                                                  StringComparer.OrdinalIgnoreCase);

    foreach (var customValue in bean.CustomValues)
    {
      var matchingColumn = customColumns.FirstOrDefault(column => column.Id == customValue.CustomColumnId.Value);
      if (matchingColumn is null)
      {
        continue;
      }

      customFields[matchingColumn.Key] = ParseCustomValue(customValue.Value, matchingColumn.DataType);
    }

    var dto = new BeanDto(
      bean.Id,
      bean.TastingProfile,
      bean.BagWeightG,
      bean.RoastIndex,
      bean.NumFarms,
      bean.NumAcidityNotes,
      bean.NumSweetnessNotes,
      bean.X,
      bean.Y,
      customFields);

    return new BeanRow(dto);
  }

  private static IReadOnlyList<CustomColumnDefinition> BuildCustomColumnDefinitions(IEnumerable<CustomColumn> customColumns)
  {
    return CustomColumnKeyBuilder.Build(customColumns)
      .Select(static definition => new CustomColumnDefinition(
        definition.Id,
        definition.Name,
        definition.Key,
        definition.DataType))
      .ToList();
  }

  private static object? ParseCustomValue(string value, CustomColumnType dataType)
  {
    return dataType switch
    {
      CustomColumnType.Integer when int.TryParse(value, out var intValue) => intValue,
      CustomColumnType.Double when double.TryParse(value, out var doubleValue) => doubleValue,
      _ => value
    };
  }

  private static string ToApiDataType(CustomColumnType dataType)
  {
    return dataType switch
    {
      CustomColumnType.Integer => "integer",
      CustomColumnType.Double => "double",
      _ => "string"
    };
  }

  private static string? NormalizeKey(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : value.Trim().Replace("-", "_").ToLowerInvariant();
  }

  private sealed record BeanRow(BeanDto Dto);

  private sealed record CustomColumnDefinition(
    Guid Id,
    string Name,
    string Key,
    CustomColumnType DataType);
}
