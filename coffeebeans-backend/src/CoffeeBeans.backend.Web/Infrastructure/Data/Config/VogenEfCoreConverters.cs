using CoffeeBeans.backend.Web.Domain.BeanAggregate;
using Vogen;

namespace CoffeeBeans.backend.Web.Infrastructure.Data.Config;

[EfCoreConverter<BeanId>]
[EfCoreConverter<CustomColumnId>]
internal partial class VogenEfCoreConverters;
