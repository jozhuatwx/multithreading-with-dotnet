using System;
using System.Threading;

namespace dotnet_multithreading
{
    public class Statistics
    {
        // initialise
        private int _numberOfServed = 0;
        private int _numberOfUnserved = 0;
        private int _numberOfPotential = 0;
        private long _totalElapsedTime = 0;

        // increment served
        public void IncrementServed()
        {
            Interlocked.Increment(ref _numberOfServed);
        }

        // increment unserved
        public void IncrementUnserved()
        {
            Interlocked.Increment(ref _numberOfUnserved);
        }

        // add potential
        public void AddPotential(int potential)
        {
            Interlocked.Add(ref _numberOfPotential, potential);
        }

        // add elapsed time
        public void AddElapsedTime(long elapsedTime)
        {
            Interlocked.Add(ref _totalElapsedTime, elapsedTime);
        }

        public int GetNumberOfServed()
        {
            return _numberOfServed;
        }

        public int GetNumberOfUnserved()
        {
            return _numberOfUnserved;
        }

        public int GetNumberOfPotential()
        {
            return _numberOfPotential;
        }

        public long GetAverageElapsedTime()
        {
            if (_numberOfServed > 0)
            {
                return _totalElapsedTime / _numberOfServed;
            }
            return 0;
        }
    }
}