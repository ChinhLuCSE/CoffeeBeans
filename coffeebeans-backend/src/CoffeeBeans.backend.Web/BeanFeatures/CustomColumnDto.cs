namespace CoffeeBeans.backend.Web.BeanFeatures;

public record CustomColumnDto(
  Guid Id,
  string Name,
  string Key,
  string DataType);
