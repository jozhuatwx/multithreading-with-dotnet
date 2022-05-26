using System;

namespace dotnet_multithreading
{
    class Program
    {
        // reaonly configurations
        // set number of each ingredient
        private static readonly int _numOfCoffee = 1;
        private static readonly int _numOfMilk = 1;
        // set number of fountain taps
        private static readonly int _numOfFountainTap = 1;

        // set clock interval
        private static readonly int _clockInterval = 1000;
        // set last order and closing time
        private static readonly int _lastCallTime = 15;
        private static readonly int _closingTime = 25;

        // set number of waiters
        private static readonly int _numOfWaiter = 1;
        // set cooldown interval
        private static readonly int _cooldownInterval = 5;

        // set number of seats
        private static readonly int _numOfSeat = 10;

        // set ratio of cappuccino against fruit juice
        private static readonly int _ratioCappuccino = 1;
        private static readonly int _ratioFruitJuice = 0;

        // initialise
        private static Statistics _statistics = new Statistics();
        private static Cupboard _cupboard = new Cupboard(_numOfCoffee, _numOfMilk);
        private static JuiceFountain _juiceFountain = new JuiceFountain(_numOfFountainTap);
        private static Table _table = new Table(_numOfSeat);

        // clock thread
        private static Clock _clock = new Clock(_clockInterval, _lastCallTime, _closingTime);

        // worker thread
        private static Waiter[] _waiters = new Waiter[_numOfWaiter];
        private static Owner _owner = new Owner(_clock, _table, _cupboard, _juiceFountain, _waiters, _cooldownInterval);
        private static Worker[] _workers = new Worker[_numOfWaiter + 1];
        
        // crowd thread
        private static Crowd _crowd = new Crowd(_clock, _table, _workers, _statistics, _ratioCappuccino, _ratioFruitJuice);

        static void Main(string[] args)
        {
            _workers[0] = _owner;
            for (var i = 0; i <= _waiters.Length; i++)
            {
                _waiters[i] = new Waiter(++i, _clock, _cupboard, _juiceFountain, _cooldownInterval);
                _workers[i] = _waiters[i - 1];
            }

            // start threads
            _clock.Start();
            foreach (var worker in _workers)
            {
                worker.Start();
            }
            _crowd.Start();

            // wait for all threads to stop
            _crowd.Join();
            _clock.Join();
            foreach (var worker in _workers)
            {
                worker.Join();
            }

            // print statistics
            Console.WriteLine($"\nTotal customers served: {_statistics.GetNumberOfServed().ToString()}");
            Console.WriteLine($"Average elapsed time: {_statistics.GetAverageElapsedTime().ToString()}ms");
            Console.WriteLine($"\nTotal customers unserved: {_statistics.GetNumberOfUnserved().ToString()}");
            Console.WriteLine($"Total customers left without a seat: {_statistics.GetNumberOfPotential().ToString()}");
        }
    }
}
