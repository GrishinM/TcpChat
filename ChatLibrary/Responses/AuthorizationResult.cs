namespace ChatLibrary.Responses
{
    public class AuthorizationResult
    {
        public bool Success { get; set; }
        
        public ClientInfo ClientInfo { get; set; }
        
        public string Message { get; set; }
    }
}