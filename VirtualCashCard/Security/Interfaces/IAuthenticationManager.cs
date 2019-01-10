using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualCashCard.Security.Interfaces
{
    public interface IAuthenticationManager
    {
        bool Authenticate(string cardNumber, string pin);
        bool IsAuthenticated();
    }
}
