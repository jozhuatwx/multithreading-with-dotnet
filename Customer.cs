using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace dotnet_multithreading
{
    public class Customer : ThreadBase
    {
        // readonly
        private readonly Order _order;
        private Clock _clock;
        private Table _table;
        private Worker[] _workers;
        private Statistics _statistics;
        // initialise
        private bool _ordered = false;

        // constructor
        public Customer(Clock clock, int id, Table table, Worker[] workers, Statistics statistics, Order order)
        {
            SetName($"Customer {id.ToString()}");
            _clock = clock;
            _table = table;
            _workers = workers;
            _statistics = statistics;
            _order = order;
        }

        public override void Run()
        {
            // set start time
            var watch = new Stopwatch();
            watch.Start();
            int index;
            int i, totalWorkers;
            // wait for turn
            do
            {
                // count number of working workers
                for (i = 1, totalWorkers = 1; i < _workers.Length; i++)
                {
                    if (_workers[i].GetWorking())
                    {
                        totalWorkers++;
                    }
                }

                index = _table.GetCustomers()
                    .TakeWhile(x => x != this).Count();

                if (index >= 0 && index < totalWorkers)
                {
                    break;
                }
            } while (!_ordered && !_clock.IsClosing());

            // ask to order
            do
            {
                // check if it is past closing time
                if (_clock.IsClosing())
                {
                    // increment the number of customers unserved
                    _statistics.IncrementUnserved();
                    break;
                }

                // try to ask works to take order
                foreach (var worker in _workers)
                {
                    if (worker.TakeOrder())
                    {
                        Console.WriteLine($"{GetName()} ordered {_order.ToString()} from {worker.GetName()}");
                        _ordered = true;
                        // asks worker to serve order
                        worker.ServeOrder(this);
                        // random drinking time
                        Console.WriteLine($"{GetName()} is drinking {_order.ToString()}");
                        Thread.Sleep(ThreadLocalRandom.Next(1, 6) * 500);
                        // stop the stopwatch
                        watch.Stop();
                        break;
                    }
                }
            } while (!_ordered);

            // leave seat
            Console.WriteLine($"{GetName()} left");
            _table.LeaveSeat(this);
            // statistics
            if (_ordered)
            {
                // count elapsed time
                _statistics.AddElapsedTime(watch.ElapsedMilliseconds);
                // increment the number of customers served
                _statistics.IncrementServed();
            }
            else
            {
                watch.Reset();
            }
        }

        public Order GetOrder()
        {
            return _order;
        }
    }
}