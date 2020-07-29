namespace ChatLibrary.Requests
{
    public class MessageRequest
    {
        public ClientInfo Sender { get; set; }

        public string Message { get; set; }
    }
}