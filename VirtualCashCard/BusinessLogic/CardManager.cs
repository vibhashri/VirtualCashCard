using System;
using System.Collections.Generic;
using System.Text;
using VirtualCashCard.BusinessLogic.Interfaces;
using VirtualCashCard.Models;
using VirtualCashCard.Repository.Interfaces;
using VirtualCashCard.Security.Interfaces;

namespace VirtualCashCard.BusinessLogic
{
    public class CardManager : ICardManager
    {
        private ICardRepository cardRepository;
        private ICardTransactionRepository cardTransactionRepository;
        private IAuthenticationManager authenticationManager;

        public CardManager(ICardRepository cardRepository, ICardTransactionRepository cardTransactionRepository, IAuthenticationManager authenticationManager)
        {
            this.cardRepository = cardRepository;
            this.cardTransactionRepository = cardTransactionRepository;
            this.authenticationManager = authenticationManager;
        }

        public bool Credit(string cardNumber, decimal amount)
        {
            if (!authenticationManager.IsAuthenticated())   //authenticationManager.Authenticate(cardNumberValue, pinValue) is called successfully before invoking credit or debit method
            {
                return false;
            }
            var card = this.GetSummary(cardNumber);
            try
            {
                cardTransactionRepository.Insert(new CardTransaction { LongNumber = cardNumber, Amount = amount, TransactionType = "Cr" });  //avoid using magic strings and replace with enums or constants
                cardRepository.Update(new Card { LongNumber = cardNumber, Balance = card.Balance + amount });
                return true;
            }
            catch (Exception ex)
            {
                //log the exception
                return false;
            }
        }

        public bool Debit(string cardNumber, decimal amount)
        {
            if (!authenticationManager.IsAuthenticated())   //authenticationManager.Authenticate(cardNumberValue, pinValue) is called successfully before invoking credit or debit method
            {
                return false;
            }
            var card = this.GetSummary(cardNumber);
            if (card.Balance < amount)
            {
                return false;
            }
            try
            {
                cardTransactionRepository.Insert(new CardTransaction { LongNumber = cardNumber, Amount = amount, TransactionType = "Db" });  //avoid using magic strings and replace with enums or constants
                cardRepository.Update(new Card { LongNumber = cardNumber, Balance = card.Balance - amount });
                return true;
            }
            catch(Exception ex)
            {
                //log the exception
                return false;
            }
        }

        public Card GetSummary(string cardNumber)
        {
            return cardRepository.GetByLongNumber(cardNumber);
        }
    }
}
