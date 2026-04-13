using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Primitives;

namespace CoffeeBeans.backend.Web.BeanFeatures.List;

public sealed class ListBeansRequest
{
  [BindFrom("page")]
  public int Page { get; init; } = 1;

  [BindFrom("per_page")]
  public int PerPage { get; init; } = Constants.DEFAULT_PAGE_SIZE;

  [BindFrom("sort_by")]
  public string? SortBy { get; init; }

  [BindFrom("sort_dir")]
  public string? SortDir { get; init; }
}

public record BeanListResponse(
  IReadOnlyList<BeanRecord> Items,
  IReadOnlyList<CustomColumnDto> CustomColumns,
  int Page,
  int PerPage,
  int TotalCount,
  int TotalPages);

public class ListEndpoint(IMediator mediator) : Endpoint<ListBeansRequest, BeanListResponse, ListBeansMapper>
{
  private static readonly HashSet<string> ReservedQueryKeys = new(StringComparer.OrdinalIgnoreCase)
  {
    "page",
    "per_page",
    "sort_by",
    "sort_dir"
  };

  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/Beans");
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "List Beans with dynamic custom fields";
      s.Description = "Reads all beans and custom column values, then applies in-memory filtering, sorting, and pagination. Query params other than page, per_page, sort_by, and sort_dir are treated as field filters, including custom_* keys.";
      s.ExampleRequest = new ListBeansRequest { Page = 1, PerPage = 10, SortBy = "custom_origin", SortDir = "asc" };
      s.ResponseExamples[200] = new BeanListResponse(
        new List<BeanRecord>
        {
          new()
          {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
            TastingProfile = "Fruity and Floral",
            BagWeightG = 250.0,
            RoastIndex = 0.75,
            NumFarms = 3,
            NumAcidityNotes = 2,
            NumSweetnessNotes = 3,
            X = 1.5,
            Y = 2.5,
            CustomFields = new Dictionary<string, object?>
            {
              ["custom_origin"] = "Ethiopia",
              ["custom_score"] = 92
            }
          }
        },
        new List<CustomColumnDto>
        {
          new(Guid.Parse("660e8400-e29b-41d4-a716-446655440001"), "Origin", "custom_origin", "string"),
          new(Guid.Parse("660e8400-e29b-41d4-a716-446655440002"), "Score", "custom_score", "integer")
        },
        1, 10, 1, 1);

      s.Params["page"] = "1-based page index (default 1)";
      s.Params["per_page"] = $"Page size 1-{Constants.MAX_PAGE_SIZE} (default {Constants.DEFAULT_PAGE_SIZE})";
      s.Params["sort_by"] = "Field name to sort by. Supports core bean fields and custom_* keys.";
      s.Params["sort_dir"] = "Sort direction: asc or desc (default asc)";
      s.Params["<any-other-query-param>"] = "Applied as a filter against the matching bean field or custom_* key.";

      s.Responses[200] = "Filtered, sorted, and paginated list of beans returned successfully";
      s.Responses[400] = "Invalid pagination or sorting parameters";
    });

    Tags("Beans");

    Description(builder => builder
      .Accepts<ListBeansRequest>()
      .Produces<BeanListResponse>(200, "application/json")
      .ProducesProblem(400));
  }

  public override async Task HandleAsync(ListBeansRequest request, CancellationToken cancellationToken)
  {
    var filters = HttpContext.Request.Query
      .Where(entry => !ReservedQueryKeys.Contains(entry.Key))
      .ToDictionary(static entry => entry.Key,
                    static entry => entry.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);

    var result = await _mediator.Send(
      new ListBeansQuery(request.Page, request.PerPage, request.SortBy, request.SortDir, filters),
      cancellationToken);

    if (!result.IsSuccess)
    {
      await Send.ErrorsAsync(statusCode: 400, cancellationToken);
      return;
    }

    var pagedResult = result.Value;
    AddLinkHeader(pagedResult.Page, pagedResult.PerPage, pagedResult.TotalPages);

    var response = Map.FromEntity(pagedResult);
    await Send.OkAsync(response, cancellationToken);
  }

  private void AddLinkHeader(int page, int perPage, int totalPages)
  {
    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

    string Link(string rel, int targetPage)
    {
      var queryValues = HttpContext.Request.Query
        .ToDictionary(static entry => entry.Key,
                      static entry => entry.Value,
                      StringComparer.OrdinalIgnoreCase);

      queryValues["page"] = new StringValues(targetPage.ToString());
      queryValues["per_page"] = new StringValues(perPage.ToString());

      var queryString = string.Join("&", queryValues.SelectMany(static entry =>
        entry.Value.Count == 0
          ? [$"{Uri.EscapeDataString(entry.Key)}="]
          : entry.Value.Select(value => $"{Uri.EscapeDataString(entry.Key)}={Uri.EscapeDataString(value ?? string.Empty)}")));

      return $"<{baseUrl}?{queryString}>; rel=\"{rel}\"";
    }

    var parts = new List<string>();
    if (page > 1)
    {
      parts.Add(Link("first", 1));
      parts.Add(Link("prev", page - 1));
    }

    if (page < totalPages)
    {
      parts.Add(Link("next", page + 1));
      parts.Add(Link("last", totalPages));
    }

    if (parts.Count > 0)
    {
      HttpContext.Response.Headers["Link"] = string.Join(", ", parts);
    }
  }
}

public sealed class ListBeansValidator : Validator<ListBeansRequest>
{
  public ListBeansValidator()
  {
    RuleFor(x => x.Page)
      .GreaterThanOrEqualTo(1)
      .WithMessage("page must be >= 1");

    RuleFor(x => x.PerPage)
      .InclusiveBetween(1, Constants.MAX_PAGE_SIZE)
      .WithMessage($"per_page must be between 1 and {Constants.MAX_PAGE_SIZE}");

    RuleFor(x => x.SortBy)
      .Must(static value => value is null || value.Trim().Length > 0)
      .WithMessage("sort_by cannot be empty when provided");

    RuleFor(x => x.SortDir)
      .Must(static value => string.IsNullOrWhiteSpace(value) ||
                            value.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                            value.Equals("desc", StringComparison.OrdinalIgnoreCase))
      .WithMessage("sort_dir must be either 'asc' or 'desc'");
  }
}

public sealed class ListBeansMapper
  : Mapper<ListBeansRequest, BeanListResponse, BeanListDto>
{
  public override BeanListResponse FromEntity(BeanListDto e)
  {
    var items = e.Items
      .Select(static p => new BeanRecord
      {
        Id = p.BeanId.Value,
        TastingProfile = p.TastingProfile,
        BagWeightG = p.BagWeightG,
        RoastIndex = p.RoastIndex,
        NumFarms = p.NumFarms,
        NumAcidityNotes = p.NumAcidityNotes,
        NumSweetnessNotes = p.NumSweetnessNotes,
        X = p.X,
        Y = p.Y,
        CustomFields = new Dictionary<string, object?>(p.CustomFields, StringComparer.OrdinalIgnoreCase)
      })
      .ToList();

    return new BeanListResponse(items, e.CustomColumns, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
  }
}
