using System.Threading;

namespace dotnet_multithreading
{
    public class Cupboard
    {
        // readonly
        private readonly SemaphoreSlim _numOfCoffee;
        private readonly SemaphoreSlim _numOfMilk;
        
        // constructor
        public Cupboard(int numOfCoffee, int numOfMilk)
        {
            this._numOfCoffee = new SemaphoreSlim(numOfCoffee, numOfCoffee);
            this._numOfMilk = new SemaphoreSlim(numOfMilk, numOfMilk);
        }

        // open the cupboard
        public void Open()
        {
            Monitor.Enter(this);
            Thread.Sleep(ThreadLocalRandom.Next(1, 4) * 100);
        }

        // close the cupboard
        public void Close()
        {
            Thread.Sleep(ThreadLocalRandom.Next(1, 4) * 100);
            Monitor.Exit(this);
        }

        // take a cup
        public void TakeCup()
        {
            Thread.Sleep(ThreadLocalRandom.Next(3, 6) * 100);
        }

        // take a glass
        public void TakeGlass()
        {
            Thread.Sleep(ThreadLocalRandom.Next(3, 6) * 100);
        }

        // take coffee
        public bool TakeCoffee()
        {
            if (_numOfCoffee.CurrentCount > 0)
            {
                _numOfCoffee.Wait();
                Thread.Sleep(ThreadLocalRandom.Next(3, 6) * 100);
                return true;
            }
            return false;
        }

        // return coffee
        public void ReturnCoffee()
        {
            Thread.Sleep(ThreadLocalRandom.Next(3, 6) * 100);
            _numOfCoffee.Release();
        }

        // take milk
        public bool TakeMilk()
        {
            if (_numOfMilk.CurrentCount > 0)
            {
                _numOfMilk.Wait();
                Thread.Sleep(ThreadLocalRandom.Next(3, 6) * 100);
                return true;
            }
            return false;
        }

        // return milk
        public void ReturnMilk()
        {
            Thread.Sleep(ThreadLocalRandom.Next(3, 6) * 100);;
            _numOfMilk.Release();
        }
    }
}