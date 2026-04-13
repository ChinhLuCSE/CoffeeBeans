using CoffeeBeans.backend.Web.BeanFeatures;
using CoffeeBeans.backend.Web.BeanFeatures.Add;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeans.backend.Web.Infrastructure.Data.Commands;

public sealed class AddColumnService(AppDbContext dbContext) : IAddColumnService
{
  private const int BatchSize = 1000;
  private readonly AppDbContext _dbContext = dbContext;

  public async Task<Result<CustomColumnDto>> AddAsync(AddColumnCommand command, CancellationToken cancellationToken)
  {
    var normalizedName = command.Name.Trim();
    var normalizedType = command.Type.Trim().ToLowerInvariant();

    if (string.IsNullOrWhiteSpace(normalizedName))
    {
      return Result.Invalid(
      [
        new ValidationError
        {
          Identifier = nameof(command.Name),
          ErrorMessage = "Column name is required"
        }
      ]);
    }

    if (!TryParseType(normalizedType, out var columnType))
    {
      return Result.Invalid(
      [
        new ValidationError
        {
          Identifier = nameof(command.Type),
          ErrorMessage = "Invalid data type"
        }
      ]);
    }

    var normalizedNameLower = normalizedName.ToLowerInvariant();
    var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

    return await executionStrategy.ExecuteAsync(async () =>
    {
      var existingColumn = await _dbContext.CustomColumns
        .AsNoTracking()
        .AnyAsync(
          column => column.ColumnName.ToLower() == normalizedNameLower,
          cancellationToken);

      if (existingColumn)
      {
        return Result.Conflict($"Column '{normalizedName}' already exists.");
      }

      await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

      var newColumn = new CustomColumn(CustomColumnId.From(Guid.NewGuid()), normalizedName, columnType);
      _dbContext.CustomColumns.Add(newColumn);
      await _dbContext.SaveChangesAsync(cancellationToken);
      var customColumnId = newColumn.Id;
      var customColumnGuid = newColumn.Id.Value;

      // Keep the backfill phase focused on the new dependent rows only.
      _dbContext.ChangeTracker.Clear();

      var beans = await _dbContext.Beans
        .AsNoTracking()
        .OrderBy(static bean => bean.TastingProfile)
        .ToListAsync(cancellationToken);

      if (!IsInitialized(customColumnId))
      {
        throw new InvalidOperationException("CustomColumnId was not initialized before backfill.");
      }

      if (beans.Any(static bean => !IsInitialized(bean.Id)))
      {
        throw new InvalidOperationException("Encountered bean rows with uninitialized BeanId values during backfill.");
      }

      for (var index = 0; index < beans.Count; index += BatchSize)
      {
        var batch = beans
          .Skip(index)
          .Take(BatchSize)
          .Select(bean => new BeanCustomValue(bean.Id, customColumnId, GenerateRandomValue(columnType)))
          .ToList();

        if (batch.Count == 0)
        {
          continue;
        }

        _dbContext.BeanCustomValues.AddRange(batch);
        await _dbContext.SaveChangesAsync(cancellationToken);
      }

      await transaction.CommitAsync(cancellationToken);

      var customColumns = await _dbContext.CustomColumns
        .AsNoTracking()
        .ToListAsync(cancellationToken);

      var keyDefinition = CustomColumnKeyBuilder.Build(customColumns)
        .Single(definition => definition.Id == customColumnGuid);

      return Result.Success(
        new CustomColumnDto(
          customColumnGuid,
          newColumn.ColumnName,
          keyDefinition.Key,
          ToApiDataType(newColumn.DataType)));
    });
  }

  private static bool TryParseType(string type, out CustomColumnType columnType)
  {
    switch (type)
    {
      case "integer":
        columnType = CustomColumnType.Integer;
        return true;
      case "double":
      case "float":
        columnType = CustomColumnType.Double;
        return true;
      case "string":
        columnType = CustomColumnType.String;
        return true;
      default:
        columnType = default;
        return false;
    }
  }

  private static string GenerateRandomValue(CustomColumnType type)
  {
    return type switch
    {
      CustomColumnType.Integer => Random.Shared.Next(1, 101).ToString(),
      CustomColumnType.Double => Math.Round(Random.Shared.NextDouble() * 100, 2).ToString("0.00"),
      _ => $"Bean-{Random.Shared.Next(1000, 9999)}"
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

  private static bool IsInitialized(BeanId beanId)
  {
    try
    {
      _ = beanId.Value;
      return true;
    }
    catch (Vogen.ValueObjectValidationException)
    {
      return false;
    }
  }

  private static bool IsInitialized(CustomColumnId customColumnId)
  {
    try
    {
      _ = customColumnId.Value;
      return true;
    }
    catch (Vogen.ValueObjectValidationException)
    {
      return false;
    }
  }
}
