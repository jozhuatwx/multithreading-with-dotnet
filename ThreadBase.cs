using System.Threading;

namespace dotnet_multithreading
{
    public abstract class ThreadBase
    {
        private Thread _thread;

        protected ThreadBase()
        {
            _thread = new Thread(Run);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Join()
        {
            _thread.Join();
        }

        public string GetName()
        {
            return _thread.Name;
        }

        public void SetName(string name)
        {
            if (_thread.Name == null)
            {
                _thread.Name = name;
            }
        }

        public void SetPriority(ThreadPriority priority)
        {
            _thread.Priority = priority;
        }

        public abstract void Run();
    }
}