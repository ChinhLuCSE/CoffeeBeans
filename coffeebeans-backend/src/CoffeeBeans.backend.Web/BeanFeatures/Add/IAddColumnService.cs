namespace CoffeeBeans.backend.Web.BeanFeatures.Add;

/// <summary>
/// Represents the infrastructure-facing service responsible for creating a custom column
/// and backfilling randomized values for all beans.
/// </summary>
public interface IAddColumnService
{
  Task<Result<CustomColumnDto>> AddAsync(AddColumnCommand command, CancellationToken cancellationToken);
}
