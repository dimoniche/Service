namespace ServiceSaleMachine.Drivers
{
    public class Message
    {
        public MessageEndPoint Recipient { get; set; }
        public string Content { get; set; }
    }
}