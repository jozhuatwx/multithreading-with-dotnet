using System;
using System.Threading;

namespace dotnet_multithreading
{
    public static class ThreadLocalRandom
    {
        // sead
        static int seed = Environment.TickCount;
        // thread-safe random
        static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Next(int minValue, int maxValue)
        {
            return random.Value.Next(minValue, maxValue);
        }
    }
}