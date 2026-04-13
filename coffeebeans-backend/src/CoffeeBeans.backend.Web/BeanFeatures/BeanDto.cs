using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.BeanFeatures;

public record BeanDto(BeanId BeanId,
        string TastingProfile,
        double BagWeightG,
        double RoastIndex,
        int NumFarms,
        int NumAcidityNotes,
        int NumSweetnessNotes,
        double X,
        double Y,
        IReadOnlyDictionary<string, object?> CustomFields);
