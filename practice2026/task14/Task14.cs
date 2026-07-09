using System;
using System.Threading;

class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double totalIntegral = 0.0;
        double range = (b - a) / threadsNumber;

        Barrier barrier = new Barrier(threadsNumber, (brier) => {
        });

        Thread[] threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int index = i; 
            threads[i] = new Thread(() =>
            {
                double start = a + index * range;
                double end = start + range;
                double localSum = 0.0;

                for (double x = start; x < end; x += step)
                {
                    localSum += (function(x) + function(Math.Min(x + step, end))) * step / 2.0;
                }

                AddToTotal(ref totalIntegral, localSum);

                barrier.SignalAndWait();
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return totalIntegral;
    }

    private static void AddToTotal(ref double location, double value)
    {
        double initialValue, newValue;
        do
        {
            initialValue = location;
            newValue = initialValue + value;
        }
        while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);
    }
}