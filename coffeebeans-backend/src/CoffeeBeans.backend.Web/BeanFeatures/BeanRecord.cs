using System.Text.Json.Serialization;

namespace CoffeeBeans.backend.Web.BeanFeatures;

public sealed class BeanRecord
{
        public Guid Id { get; init; }
        public string TastingProfile { get; init; } = default!;
        public double BagWeightG { get; init; }
        public double RoastIndex { get; init; }
        public int NumFarms { get; init; }
        public int NumAcidityNotes { get; init; }
        public int NumSweetnessNotes { get; init; }
        public double X { get; init; }
        public double Y { get; init; }

        [JsonExtensionData]
        public IDictionary<string, object?> CustomFields { get; init; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
