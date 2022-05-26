using System;
using System.Threading;

namespace dotnet_multithreading
{
    public class Waiter : Worker
    {
        // constructor
        public Waiter(int id, Clock clock, Cupboard cupboard, JuiceFountain juiceFountain, int cooldownInterval)
          : base(id, clock, cupboard, juiceFountain, cooldownInterval)
        {
            SetName($"Waiter {id.ToString()}");
            SetPriority(ThreadPriority.AboveNormal);
        }

        public override void Run()
        {
            lock (GetClock())
            {
                // wait for last order time
                Monitor.Wait(GetClock());
                // wait for closing time
                Monitor.Wait(GetClock());
            }

            // finish current serve and prevent future serve
            Monitor.Enter(this);
            // leave café
            Console.WriteLine($"{GetName()} left");
            // set as no longer working
            SetWorking(false);
        }
    }
}