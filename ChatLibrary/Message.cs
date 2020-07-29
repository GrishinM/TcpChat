namespace ChatLibrary
{
    public class Message
    {
        public ClientInfo Sender { get; set; }
        
        public string ReceiverLogin { get; set; }
        
        public string Text { get; set; }
    }
}