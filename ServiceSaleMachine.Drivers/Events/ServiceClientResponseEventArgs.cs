using ServiceSaleMachine.Drivers;
using System;

namespace ServiceSaleMachine.Drivers
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
