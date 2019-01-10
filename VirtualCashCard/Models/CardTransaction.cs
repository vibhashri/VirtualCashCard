using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualCashCard.Models
{
    public class CardTransaction
    {
        //Ideally it has foreign key relation between Card and CardTransaction but here did not consider any

        public string LongNumber { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
    }
}
