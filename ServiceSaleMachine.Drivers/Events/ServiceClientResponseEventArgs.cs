using AirVitamin.Drivers;
using System;

namespace AirVitamin.Drivers
{
    public class ServiceClientResponseEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public ServiceClientResponseEventArgs(Message message)
        {
            Message = message;
        }
    }
}
