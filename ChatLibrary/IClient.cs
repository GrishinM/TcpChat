using System;

namespace ChatLibrary
{
    public interface IClient
    {
        event Action AuthorizationSucceeded;
        event Action<string> AuthorizationFailed;
        event Action RegistrationSucceeded;
        event Action<string> RegistrationFailed;
        event Action Message;
    }
}