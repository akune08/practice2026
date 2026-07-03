using Xunit;
public class CalculatorTests
{
    [Fact]
    public void TestCalculatorOperations()
    {
        ICalculator calc = DynamicCalculatorFactory.Create();

        Assert.Equal(5, calc.Add(2, 3));
        Assert.Equal(1, calc.Minus(3, 2));
        Assert.Equal(6, calc.Mul(2, 3));
        Assert.Equal(2, calc.Div(6, 3));
    }
}