namespace CoffeeBeans.backend.Web.BeanFeatures.List;

public record BeanListDto(
  IReadOnlyList<BeanDto> Items,
  IReadOnlyList<CustomColumnDto> CustomColumns,
  int Page,
  int PerPage,
  int TotalCount,
  int TotalPages);
