using System.Threading;

namespace dotnet_multithreading
{
    public class Clock : ThreadBase
    {
        // readonly
        private readonly int _clockInterval;
        private readonly int _lastOrderTime;
        private readonly int _closingTime;
        // initialise
        private int _time = 0;

        // constructor
        public Clock(int clockInterval, int lastOrderTime, int closingTime)
        {
            _clockInterval = clockInterval;
            _lastOrderTime = lastOrderTime;
            _closingTime = closingTime;
        }

        public override void Run()
        {
            // operating time
            for (; _time < _lastOrderTime; _time++)
            {
                Thread.Sleep(_clockInterval);
            }

            lock (this)
            {
                // notify last call
                Monitor.PulseAll(this);
            }

            // last order time
            for (; _time < _closingTime; _time++)
            {
                Thread.Sleep(_clockInterval);
            }

            lock (this)
            {
                // notify closing
                Monitor.PulseAll(this);
            }
        }

        // check if past last order
        public bool IsLastOrder()
        {
            return _time >= _lastOrderTime;
        }

        // check if past closing
        public bool IsClosing()
        {
            return _time >= _closingTime;
        }
    }
}
