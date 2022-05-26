using System;
using System.Threading;

namespace dotnet_multithreading
{
    public class Crowd : ThreadBase
    {
        // readonly
        private readonly int _ratioCappuccino;
        private readonly int _ratioFruitJuice;
        private Clock _clock;
        private Table _table;
        private Worker[] _workers;
        private Statistics _statistics;
        // initialise
        private int _id = 1;
        
        // constructor
        public Crowd(Clock clock, Table table, Worker[] workers, Statistics statistics, int ratioCappuccino, int ratioFruitJuice)
        {
            SetName("Crowd");
            _clock = clock;
            _table = table;
            _workers = workers;
            _statistics = statistics;
            _ratioCappuccino = ratioCappuccino;
            _ratioFruitJuice = ratioFruitJuice;
        }

        public override void Run()
        {
            while (!_clock.IsLastOrder())
            {
                // random number of customers
                var numberOfCustomers = ThreadLocalRandom.Next(1, 6);
                // allocate sets for customers
                for (; numberOfCustomers > 0 && _table.GetNumOfAvailableSeat() > 0; numberOfCustomers--, _id++)
                {
                    Customer customer = new Customer(_clock, _id, _table, _workers, _statistics, DrinkRatio());
                    _table.TakeSeat(customer);
                    customer.Start();
                    Console.WriteLine($"{customer.GetName()} is seated");
                }
                // announce that there are no seats left
                if (numberOfCustomers > 0)
                {
                    Console.WriteLine("No seats left");
                    _statistics.AddPotential(numberOfCustomers);
                }
                // random interval to next batch
                Thread.Sleep(ThreadLocalRandom.Next(1, 6) * 500);
            }
        }

        // set customer order
        private Order DrinkRatio()
        {
            // randomly select a number
            var random = ThreadLocalRandom.Next(1, _ratioFruitJuice + _ratioCappuccino + 1);

            if (random <= _ratioCappuccino)
            {
                // set order as cappuccino
                return Order.Cappuccino;
            }
            // set order as fruit juice
            return Order.FruitJuice;
        }
    }
}