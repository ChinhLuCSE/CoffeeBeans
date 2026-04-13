namespace CoffeeBeans.backend.Web.BeanFeatures.Add;

public sealed record AddColumnCommand(string Name, string Type) : ICommand<Result<CustomColumnDto>>;

public sealed class AddColumnHandler(IAddColumnService addColumnService) : ICommandHandler<AddColumnCommand, Result<CustomColumnDto>>
{
  private readonly IAddColumnService _addColumnService = addColumnService;

  public async ValueTask<Result<CustomColumnDto>> Handle(AddColumnCommand command, CancellationToken cancellationToken)
  {
    return await _addColumnService.AddAsync(command, cancellationToken);
  }
}
