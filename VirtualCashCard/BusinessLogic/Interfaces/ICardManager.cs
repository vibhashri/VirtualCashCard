using System;
using System.Collections.Generic;
using System.Text;
using VirtualCashCard.Models;

namespace VirtualCashCard.BusinessLogic.Interfaces
{
    public interface ICardManager
    {
        bool Credit(string cardNumber, decimal amount);
        bool Debit(string cardNumber, decimal amount);
        Card GetSummary(string cardNumber);
    }
}
