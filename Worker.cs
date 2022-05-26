using System;
using System.Threading;

namespace dotnet_multithreading
{
    public abstract class Worker : ThreadBase
    {
        // readonly
        private readonly Clock _clock;
        private readonly Cupboard _cupboard;
        private readonly JuiceFountain _juiceFountain;
        private readonly int _cooldownInterval;
        private int _id;
        // initialise
        private int _cooldown = 1;
        private bool _working = true;

        // constructor
        public Worker(int id, Clock clock, Cupboard cupboard, JuiceFountain juiceFountain, int cooldownInterval)
        {
            _id = id;
            _clock = clock;
            _cupboard = cupboard;
            _juiceFountain = juiceFountain;
            _cooldownInterval = cooldownInterval;
        }

        // take customers' order
        public bool TakeOrder()
        {
            return Monitor.TryEnter(this);
        }

        // serve customers' order
        public void ServeOrder(Customer customer)
        {
            switch (customer.GetOrder())
            {
                // cappuccino
                case Order.Cappuccino:
                    ServeCappuccino();
                    Console.WriteLine($"{GetName()} served {customer.GetName()}");
                    break;

                // fruit juice
                case Order.FruitJuice:
                    ServeFruitJuice();
                    Console.WriteLine($"{GetName()} served {customer.GetName()}");
                    break;


                default:
                    break;
            }
            Monitor.Exit(this);
        }
        
        // serve cappucino
        public void ServeCappuccino()
        {
            bool checkCoffee = false, checkMilk = false;
            // open cupboard
            _cupboard.Open();
            // take a cup
            _cupboard.TakeCup();
            Console.WriteLine($"{GetName()} took a cup");

            // take coffee and milk
            do
            {
                // reduce speed
                if (!_clock.IsLastOrder())
                {
                    // set wait time to prioritise number of executions and id
                    Thread.Sleep(_cooldown + (_id * 10));
                }

                // open cupboard if closed
                if (!Monitor.IsEntered(_cupboard))
                {
                    _cupboard.Open();
                }

                // check if ingredients are available
                checkCoffee = _cupboard.TakeCoffee();
                checkMilk = _cupboard.TakeMilk();

                if (checkCoffee)
                {
                    Console.WriteLine($"{GetName()} took coffee");
                }

                if (checkMilk)
                {
                    Console.WriteLine($"{GetName()} took milk");
                }

                if (checkCoffee && checkMilk)
                {
                    // close cupboard
                    _cupboard.Close();
                    // increate wait time
                    _cooldown += _cooldownInterval;
                    break;
                }
                else if (checkCoffee)
                {
                    // return unused cofee
                    Console.WriteLine($"{GetName()} returned unused coffee");
                    _cupboard.ReturnCoffee();
                }
                else if (checkMilk)
                {
                    // return used milk
                    Console.WriteLine($"{GetName()} returned usued milk");
                    _cupboard.ReturnMilk();
                }
                _cupboard.Close();

                // decrease wait time
                if (_cooldown > _cooldownInterval)
                {
                    _cooldown -= _cooldownInterval;
                }
            } while (!checkCoffee || !checkMilk);

            // pour ingredients
            Console.WriteLine($"{GetName()} pouring ingredients");
            Thread.Sleep(ThreadLocalRandom.Next(10, 21) * 100);
            
            // open cupboard
            _cupboard.Open();
            // return ingredients
            _cupboard.ReturnCoffee();
            Console.WriteLine($"{GetName()} return coffee");
            _cupboard.ReturnMilk();
            Console.WriteLine($"{GetName()} return milk");
            // close cupboard
            _cupboard.Close();
        }

        // serve fruit juice
        public void ServeFruitJuice()
        {
            bool checkTap = false;
            // open cupboard
            _cupboard.Open();
            // take a glass
            _cupboard.TakeGlass();
            Console.WriteLine($"{GetName()} took a glass");

            // close cupboard
            _cupboard.Close();

            // use juice fountain
            do
            {
                // reduce speed
                if (!_clock.IsLastOrder())
                {
                    // set wait time to prioritise number of executions and id
                    Thread.Sleep(_cooldown + (_id * 10));
                }

                // check if tap is available
                checkTap = _juiceFountain.OpenTap();
                if (checkTap)
                {
                    Console.WriteLine($"{GetName()} opened juice fountain tap");
                }

                // decrease wait time
                if (_cooldown > _cooldownInterval)
                {
                    _cooldown -= _cooldownInterval;
                }
            } while (!checkTap);

            // fill the glass
            Console.WriteLine($"{GetName()} filling glass");
            Thread.Sleep(ThreadLocalRandom.Next(10, 16) * 100);

            // close tap
            Console.WriteLine($"{GetName()} closed juice fountain tap");
            _juiceFountain.CloseTap();
        }

        // check if worker is working
        public bool GetWorking()
        {
            return _working;
        }

        public void SetWorking(bool working)
        {
            _working = working;
        }

        public Clock GetClock()
        {
            return _clock;
        }
    }
}