using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CoffeeBeans.backend.Web.Domain.BeanAggregate;

namespace CoffeeBeans.backend.Web.Infrastructure.Data;

public static class SeedData
{
    private const string DATA_URL =
        "https://raw.githubusercontent.com/techprep-gh/coffee-bean-library/main/coffee_beans_500.json";

    public static async Task InitializeAsync(AppDbContext dbContext, ILogger logger)
    {
        if (await dbContext.Beans.AnyAsync())
        {
            logger.LogInformation("Bean DB has data - seeding not required.");
            return;
        }

        await PopulateBeanDataAsync(dbContext, logger);
    }

    public static async Task PopulateBeanDataAsync(AppDbContext dbContext, ILogger logger)
    {
        logger.LogInformation("Seeding Beans from JSON source...");

        using var http = new HttpClient();

        var json = await http.GetStringAsync(DATA_URL);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var rawData = JsonSerializer.Deserialize<List<CoffeeBeanDto>>(json, options);

        if (rawData is null || rawData.Count == 0)
        {
            logger.LogWarning("No bean data found in JSON.");
            return;
        }

        var beans = rawData.Select(x => new Bean(
            BeanId.From(Guid.NewGuid()),
            x.TastingProfile,
            x.BagWeightG,
            x.RoastIndex,
            x.NumFarms,
            x.NumAcidityNotes,
            x.NumSweetnessNotes,
            x.X,
            x.Y
        )).ToList();

        await dbContext.Beans.AddRangeAsync(beans);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} Beans successfully.", beans.Count);
    }

    // DTO để map JSON
    private class CoffeeBeanDto
    {
        [JsonPropertyName("tasting_profile")]
        public string TastingProfile { get; set; } = default!;

        [JsonPropertyName("bag_weight_g")]
        public double BagWeightG { get; set; }

        [JsonPropertyName("roast_index")]
        public double RoastIndex { get; set; }

        [JsonPropertyName("num_farms")]
        public int NumFarms { get; set; }

        [JsonPropertyName("num_acidity_notes")]
        public int NumAcidityNotes { get; set; }

        [JsonPropertyName("num_sweetness_notes")]
        public int NumSweetnessNotes { get; set; }

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }
    }
}
