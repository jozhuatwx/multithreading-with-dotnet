using System;
using System.Threading;

namespace dotnet_multithreading
{
    public class Owner : Worker
    {
        private Table _table;
        private Waiter[] _waiters;

        // constructor
        public Owner(Clock clock, Table table, Cupboard cupboard, JuiceFountain juiceFountain, Waiter[] waiters, int cooldownInterval)
          : base(0, clock, cupboard, juiceFountain, cooldownInterval)
        {
            SetName("Owner");
            SetPriority(ThreadPriority.Highest);
            _table = table;
            _waiters = waiters;
        }

        public override void Run()
        {
            lock (GetClock())
            {
                // wait for last order time
                Monitor.Wait(GetClock());
            }
            // announce last order
            Console.WriteLine($"{GetName()}: Last order!");
            lock (GetClock())
            {
                // wait for closing time
                Monitor.Wait(GetClock());
            }
            // announce closing
            Console.WriteLine($"{GetName()}: Closing!");

            // finish current serve and prevent future serve
            Monitor.Enter(this);

            // wait for all waiters to leave
            foreach (var waiter in _waiters)
            {
                waiter.Join();
            }

            // wait for all customers to leave
            foreach (var customer in _table.GetCustomers())
            {
                customer.Join();
            }

            // leave café
            Console.WriteLine($"{GetName()} left");
            // set as no longer working
            SetWorking(false);
        }
    }
}