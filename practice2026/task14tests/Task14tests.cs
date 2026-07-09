using Xunit;
using System;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_LinearFunction_ReturnsCorrectResult()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4); 
    }


    [Fact]
    public void Solve_SinFunction_ReturnsCorrectResult()
    {
        var SIN = (double x) => Math.Sin(x);
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Solve_LinearRange0to5_ReturnsCorrectResult()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 5);
    }

    [Fact]
    public void Solve_ConsistencyAcrossThreadCounts()
    {
        var square = (double x) => x * x;
        double res1 = DefiniteIntegral.Solve(0, 2, square, 1e-4, 1);
        double res4 = DefiniteIntegral.Solve(0, 2, square, 1e-4, 4);

        Assert.Equal(res1, res4, 3);
    }
}