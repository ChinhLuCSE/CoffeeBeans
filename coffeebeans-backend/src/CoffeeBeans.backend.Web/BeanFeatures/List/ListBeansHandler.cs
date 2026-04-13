namespace CoffeeBeans.backend.Web.BeanFeatures.List;

public record ListBeansQuery(
  int? Page = 1,
  int? PerPage = Constants.DEFAULT_PAGE_SIZE,
  string? SortBy = null,
  string? SortDir = null,
  IReadOnlyDictionary<string, string>? Filters = null)
  : IQuery<Result<BeanListDto>>;

public class ListBeansHandler : IQueryHandler<ListBeansQuery, Result<BeanListDto>>
{
  private readonly IListBeansQueryService _query;

  public ListBeansHandler(IListBeansQueryService query)
  {
    _query = query;
  }

  public async ValueTask<Result<BeanListDto>> Handle(ListBeansQuery request,
                                                                 CancellationToken cancellationToken)
  {
    var normalizedRequest = request with
    {
      Page = request.Page ?? 1,
      PerPage = request.PerPage ?? Constants.DEFAULT_PAGE_SIZE,
      Filters = request.Filters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    };
    var result = await _query.ListAsync(normalizedRequest, cancellationToken);

    return Result.Success(result);
  }
}
