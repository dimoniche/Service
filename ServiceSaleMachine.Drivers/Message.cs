namespace ServiceSaleMachine.Drivers
{
    public class Message
    {
        public MessageEndPoint Recipient { get; set; }
        public object Content { get; set; }
    }
}