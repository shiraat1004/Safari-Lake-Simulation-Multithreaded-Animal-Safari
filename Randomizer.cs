using System;
using System.Threading;

namespace Safari
{
    public static class Randomizer
    {
        private static Random _random = new Random();
        private static object _lock = new object();
        private static double _nextGaussian = 0.0;
        private static bool _useLast = false;

        // Generates a random number following a normal distribution
        // mean (μ): The average value
        // stdDev (σ): The standard deviation (how spread out the values are)
        public static int NextGaussian(double mean, double stdDev)
        {
            lock (_lock)
            {
                if (_useLast)
                {
                    _useLast = false;
                    return (int)Math.Max(0, mean + _nextGaussian * stdDev);
                }

                double u1 = _random.NextDouble();
                double u2 = _random.NextDouble();

                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                _nextGaussian = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
                _useLast = true;

                return (int)Math.Max(0, mean + randStdNormal * stdDev);
            }
        }
    }
}