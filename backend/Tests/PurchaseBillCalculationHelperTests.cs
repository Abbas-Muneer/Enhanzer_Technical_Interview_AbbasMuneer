using Enhanzer.Api.Helpers;

namespace Enhanzer.Api.Tests;

public class PurchaseBillCalculationHelperTests
{
    [Fact]
    public void CalculationHelper_ReturnsExpectedFinancialValues()
    {
        var margin = PurchaseBillCalculationHelper.CalculateMargin(100m, 150m);
        var totalCost = PurchaseBillCalculationHelper.CalculateTotalCost(100m, 5m, 20m);
        var totalSelling = PurchaseBillCalculationHelper.CalculateTotalSelling(150m, 5m);

        Assert.Equal(50m, margin);
        Assert.Equal(400m, totalCost);
        Assert.Equal(750m, totalSelling);
    }
}
