using System;

namespace BirdTracks.Game.Core
{
    public class ServiceInitializeResult
    {
        public IService Service { get; private set; }

        public Exception Exception { get; private set; }

        public bool IsSuccessful { get { return Exception == null; } }

        public bool IsFaulted { get { return Exception != null; } }


        public ServiceInitializeResult(IService service)
        {
            Service = service;
            Exception = null;
        }

        public ServiceInitializeResult(IService service, Exception exception)
        {
            Service = service;
            Exception = exception;
        }


        public void ThrowIfFaulted()
        {
            if (IsFaulted)
            {
                throw Exception;
            }
        }
    }
}