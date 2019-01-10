using System;
using System.Collections.Generic;
using System.Text;
using VirtualCashCard.Models;

namespace VirtualCashCard.Repository.Interfaces
{
    public interface ICardRepository : IRepository<Card>
    {
        Card GetByLongNumber(string longNumber);
    }
}
