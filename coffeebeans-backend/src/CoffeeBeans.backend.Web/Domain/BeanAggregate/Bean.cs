namespace CoffeeBeans.backend.Web.Domain.BeanAggregate;

public class Bean : EntityBase<Bean, BeanId>, IAggregateRoot
{
    public string TastingProfile { get; private set; } = default!;
    public double BagWeightG { get; private set; }
    public double RoastIndex { get; private set; }
    public int NumFarms { get; private set; }
    public int NumAcidityNotes { get; private set; }
    public int NumSweetnessNotes { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    private readonly List<BeanCustomValue> _customValues = new();
    public IReadOnlyList<BeanCustomValue> CustomValues => _customValues.AsReadOnly();
    
    private Bean() { }
    public Bean(
        BeanId beanId,
        string tastingProfile,
        double bagWeightG,
        double roastIndex,
        int numFarms,
        int numAcidityNotes,
        int numSweetnessNotes,
        double x,
        double y)
    {
        // if (string.IsNullOrWhiteSpace(beanId))
        //     throw new ArgumentException("BeanId is required.", nameof(beanId));

        if (string.IsNullOrWhiteSpace(tastingProfile))
            throw new ArgumentException("TastingProfile is required.", nameof(tastingProfile));

        if (bagWeightG <= 0)
            throw new ArgumentOutOfRangeException(nameof(bagWeightG));

        if (numFarms < 0)
            throw new ArgumentOutOfRangeException(nameof(numFarms));

        if (numAcidityNotes < 0)
            throw new ArgumentOutOfRangeException(nameof(numAcidityNotes));

        if (numSweetnessNotes < 0)
            throw new ArgumentOutOfRangeException(nameof(numSweetnessNotes));

        Id = beanId;
        TastingProfile = tastingProfile;
        BagWeightG = bagWeightG;
        RoastIndex = roastIndex;
        NumFarms = numFarms;
        NumAcidityNotes = numAcidityNotes;
        NumSweetnessNotes = numSweetnessNotes;
        X = x;
        Y = y;
    }

}