namespace ServiceSaleMachine.Drivers
{
    public class Message
    {
        public DeviceEvent Event { get; set; }
        public object Content { get; set; }

        public Message()
        {
            Event = new DeviceEvent();
        }
    }
}