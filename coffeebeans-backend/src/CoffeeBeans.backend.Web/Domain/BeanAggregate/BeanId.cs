using Vogen;

namespace CoffeeBeans.backend.Web.Domain.BeanAggregate;

[ValueObject<Guid>]
public readonly partial struct BeanId
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("BeanId must set to non-default value.");
}

