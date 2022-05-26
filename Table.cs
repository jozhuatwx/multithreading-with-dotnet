using System.Collections.Generic;
using System.Threading;

namespace dotnet_multithreading
{
    public class Table
    {
        // readonly
        private readonly SemaphoreSlim _numOfSeat;
        // initialise
        private LinkedList<Customer> _customers = new LinkedList<Customer>();
        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        // constructor
        public Table(int numOfSeat)
        {
            _numOfSeat = new SemaphoreSlim(numOfSeat);
        }

        // take a seat
        public void TakeSeat(Customer customer)
        {
            _numOfSeat.Wait();
            Thread.Sleep(ThreadLocalRandom.Next(1, 3) * 100);

            // acquire write lock
            _readerWriterLock.EnterWriteLock();
            try
            {
                // add customer to list
                _customers.AddLast(customer);
            }
            finally
            {
                // release write lock
                _readerWriterLock.ExitWriteLock();
            }
        }

        // leave a seat
        public void LeaveSeat(Customer customer)
        {
            // acquire write lock
            _readerWriterLock.EnterWriteLock();
            try
            {
                // remove customer from list
                _customers.Remove(customer);
            }
            finally
            {
                // release write lock
                _readerWriterLock.ExitWriteLock();
            }

            Thread.Sleep(ThreadLocalRandom.Next(1, 3) * 100);
            _numOfSeat.Release();
        }


        public LinkedList<Customer> GetCustomers()
        {
            // acquire read lock
            _readerWriterLock.EnterReadLock();
            // clone customers
            var cloneCustomers = new LinkedList<Customer>(_customers);
            // release read lock
            _readerWriterLock.ExitReadLock();
            return cloneCustomers;
        }

        public int GetNumOfAvailableSeat()
        {
            return _numOfSeat.CurrentCount;
        }
    }
}