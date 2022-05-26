using System.Threading;

namespace dotnet_multithreading
{
    public class JuiceFountain
    {
        // readonly
        private readonly SemaphoreSlim _numOfFountainTap;

        // constructor
        public JuiceFountain(int numOfFountainTap)
        {
            _numOfFountainTap = new SemaphoreSlim(numOfFountainTap, numOfFountainTap);
        }

        // open the tap
        public bool OpenTap()
        {
            if (_numOfFountainTap.CurrentCount > 0)
            {
                _numOfFountainTap.Wait();
                Thread.Sleep(ThreadLocalRandom.Next(2, 5) * 100);
                return true;
            }
            return false;
        }

        // close the tap
        public void CloseTap()
        {
            Thread.Sleep(ThreadLocalRandom.Next(2, 5) * 100);
            _numOfFountainTap.Release();
        }
    }
}