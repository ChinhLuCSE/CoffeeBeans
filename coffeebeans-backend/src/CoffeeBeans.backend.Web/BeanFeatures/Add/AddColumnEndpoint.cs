using CoffeeBeans.backend.Web.Domain.BeanAggregate;
using FastEndpoints;
using FluentValidation;

namespace CoffeeBeans.backend.Web.BeanFeatures.Add;

public sealed class AddColumnRequest
{
  public string Name { get; init; } = string.Empty;
  public string Type { get; init; } = string.Empty;
}

public sealed record AddColumnResponse(
  string Message,
  CustomColumnDto Column);

public sealed class AddColumnEndpoint(IMediator mediator) : Endpoint<AddColumnRequest, AddColumnResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post("/Columns");
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Add a custom bean column";
      s.Description = "Creates a custom column and backfills randomized values for every existing bean.";
      s.ExampleRequest = new AddColumnRequest
      {
        Name = "brew_score",
        Type = "integer"
      };

      s.ResponseExamples[201] = new AddColumnResponse(
        "Column brew_score added successfully",
        new CustomColumnDto(
          Guid.Parse("660e8400-e29b-41d4-a716-446655440003"),
          "brew_score",
          "custom_brew_score",
          "integer"));

      s.Responses[201] = "Column created and randomized values backfilled successfully";
      s.Responses[400] = "Invalid request";
      s.Responses[409] = "A column with the same name already exists";
    });

    Tags("Columns");

    Description(builder => builder
      .Accepts<AddColumnRequest>("application/json")
      .Produces<AddColumnResponse>(201, "application/json")
      .ProducesProblem(400)
      .ProducesProblem(409));
  }

  public override async Task HandleAsync(AddColumnRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new AddColumnCommand(request.Name, request.Type),
      cancellationToken);

    switch (result.Status)
    {
      case ResultStatus.Ok:
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        await HttpContext.Response.WriteAsJsonAsync(
          new AddColumnResponse(
            $"Column {result.Value.Name} added successfully",
            result.Value),
          cancellationToken);
        return;

      case ResultStatus.Invalid:
        foreach (var validationError in result.ValidationErrors)
        {
          AddError(validationError.Identifier ?? string.Empty, validationError.ErrorMessage);
        }

        await Send.ErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
        return;

      case ResultStatus.Conflict:
        var conflictMessage = result.Errors.FirstOrDefault() ?? "A column with the same name already exists.";
        AddError(nameof(request.Name), conflictMessage);
        await Send.ErrorsAsync(StatusCodes.Status409Conflict, cancellationToken);
        return;

      default:
        ThrowError(string.Join("; ", result.Errors.DefaultIfEmpty("Failed to add column.")));
        return;
    }
  }
}

public sealed class AddColumnValidator : Validator<AddColumnRequest>
{
  private static readonly string[] AllowedTypes = ["integer", "double", "string"];

  public AddColumnValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Column name is required")
      .MaximumLength(200)
      .WithMessage("Column name must be 200 characters or fewer")
      .Matches("^[a-zA-Z0-9_]+$")
      .WithMessage("Column name can only contain letters, numbers, and underscores");

    RuleFor(x => x.Type)
      .Must(static value => AllowedTypes.Contains(value, StringComparer.OrdinalIgnoreCase))
      .WithMessage($"Data type must be one of: {string.Join(", ", AllowedTypes)}");
  }
}
