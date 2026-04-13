namespace CoffeeBeans.backend.Web.BeanFeatures.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListBeansQueryService
{
  Task<BeanListDto> ListAsync(ListBeansQuery query, CancellationToken cancellationToken);
}
