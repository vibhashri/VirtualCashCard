using Moq;
using System;
using VirtualCashCard.BusinessLogic;
using VirtualCashCard.Models;
using VirtualCashCard.Repository.Interfaces;
using VirtualCashCard.Security.Interfaces;
using Xunit;

namespace VirtualCashCard.Tests
{
    public class CardManagerTests
    {
        private Mock<ICardRepository>  mockCardRepository = new Mock<ICardRepository>();
        private Mock<ICardTransactionRepository> mockCardTransactionRepository = new Mock<ICardTransactionRepository>();
        private Mock<IAuthenticationManager> mockIAuthenticationManager = new Mock<IAuthenticationManager>();
        private CardManager cardManager;
        private const string validCardNumber = "1234";
        private Card validCard = new Card { LongNumber = validCardNumber, Balance = 1000 };

        public CardManagerTests()
        {
            cardManager = new CardManager(mockCardRepository.Object, mockCardTransactionRepository.Object, mockIAuthenticationManager.Object);
        }

        [Fact]
        public void GetSummary_ShouldCallCardRepositoryAndReturnValidSummary()
        {
            //Arrange
            mockCardRepository.Setup(x => x.GetByLongNumber(validCardNumber)).Returns(validCard);

            //Act
            var card = cardManager.GetSummary(validCardNumber);

            //Assert
            mockCardRepository.Verify(x => x.GetByLongNumber(validCardNumber), Times.Once);
            Assert.Equal(validCard.LongNumber, card.LongNumber);
            Assert.Equal(validCard.Balance, card.Balance);
        }

        [Fact]
        public void Credit_ShouldNotCallRepositoryAndReturnFalseWhenNotAuthenticated()
        {
            //Arrange
            mockCardRepository.Setup(x => x.GetByLongNumber(validCardNumber)).Returns(validCard);
            mockIAuthenticationManager.Setup(x => x.IsAuthenticated()).Returns(false);
            mockCardRepository.Setup(x => x.Update(It.IsAny<Card>()));
            mockCardTransactionRepository.Setup(x => x.Insert(It.IsAny<CardTransaction>()));
            decimal creditAmount = 100.00M;

            //Act
            var status = cardManager.Credit(validCardNumber, creditAmount);

            //Assert
            mockCardRepository.Verify(x => x.Update(It.IsAny<Card>()), Times.Never);
            mockCardTransactionRepository.Verify(x => x.Insert(It.IsAny<CardTransaction>()), Times.Never);
            mockIAuthenticationManager.Verify(x => x.IsAuthenticated(), Times.Once);
            Assert.False(status);
        }

        [Fact]
        public void Credit_ShouldCallRepositoryWhenAuthenticated()
        {
            //Arrange
            mockCardRepository.Setup(x => x.GetByLongNumber(validCardNumber)).Returns(validCard);
            mockIAuthenticationManager.Setup(x => x.IsAuthenticated()).Returns(true);
            mockCardRepository.Setup(x => x.Update(It.IsAny<Card>()));
            mockCardTransactionRepository.Setup(x => x.Insert(It.IsAny<CardTransaction>()));
            decimal creditAmount = 100.00M;

            //Act
            var status = cardManager.Credit(validCardNumber, creditAmount);

            //Assert
            mockCardRepository.Verify(x => x.Update(It.Is<Card>(c => c.LongNumber == validCardNumber && c.Balance == validCard.Balance + creditAmount)), Times.Once);
            mockCardTransactionRepository.Verify(x => x.Insert(It.Is<CardTransaction>(ct => ct.LongNumber == validCardNumber && ct.Amount == creditAmount)), Times.Once);
            mockIAuthenticationManager.Verify(x => x.IsAuthenticated(), Times.Once);
            Assert.True(status);
        }


        [Fact]
        public void Debit_ShouldNotCallRepositoryAndReturnFalseWhenNotAuthenticated()
        {
            //Arrange
            mockCardRepository.Setup(x => x.GetByLongNumber(validCardNumber)).Returns(validCard);
            mockIAuthenticationManager.Setup(x => x.IsAuthenticated()).Returns(false);
            mockCardRepository.Setup(x => x.Update(It.IsAny<Card>()));
            mockCardTransactionRepository.Setup(x => x.Insert(It.IsAny<CardTransaction>()));
            decimal  debitAmount = 100.00M;

            //Act
            var status = cardManager.Debit(validCardNumber, debitAmount);

            //Assert
            mockCardRepository.Verify(x => x.Update(It.IsAny<Card>()), Times.Never);
            mockCardTransactionRepository.Verify(x => x.Insert(It.IsAny<CardTransaction>()), Times.Never);
            mockIAuthenticationManager.Verify(x => x.IsAuthenticated(), Times.Once);
            Assert.False(status);
        }


        [Fact]
        public void Debit_ShouldCallRepositoryWhenAuthenticated()
        {
            //Arrange
            mockCardRepository.Setup(x => x.GetByLongNumber(validCardNumber)).Returns(validCard);
            mockIAuthenticationManager.Setup(x => x.IsAuthenticated()).Returns(true);
            mockCardRepository.Setup(x => x.Update(It.IsAny<Card>()));
            mockCardTransactionRepository.Setup(x => x.Insert(It.IsAny<CardTransaction>()));
            decimal debitAmount = 100.00M;

            //Act
            var status = cardManager.Debit(validCardNumber, debitAmount);

            //Assert
            mockCardRepository.Verify(x => x.Update(It.Is<Card>(c => c.LongNumber == validCardNumber && c.Balance == validCard.Balance - debitAmount)), Times.Once);
            mockCardTransactionRepository.Verify(x => x.Insert(It.Is<CardTransaction>(ct => ct.LongNumber == validCardNumber && ct.Amount == debitAmount)), Times.Once);
            mockIAuthenticationManager.Verify(x => x.IsAuthenticated(), Times.Once);
            Assert.True(status);
        }


        [Fact]
        public void Debit_ShouldNotUpdateRepositoryAndReturnFalseWhenAuthenticatedAndAmountIsMoreThanAvailableBalance()
        {
            //Arrange
            mockCardRepository.Setup(x => x.GetByLongNumber(validCardNumber)).Returns(validCard);
            mockIAuthenticationManager.Setup(x => x.IsAuthenticated()).Returns(true);
            mockCardRepository.Setup(x => x.Update(It.IsAny<Card>()));
            mockCardTransactionRepository.Setup(x => x.Insert(It.IsAny<CardTransaction>()));
            decimal debitAmount = 1100.00M;

            //Act
            var status = cardManager.Debit(validCardNumber, debitAmount);

            //Assert
            mockCardRepository.Verify(x => x.GetByLongNumber(validCardNumber), Times.Once);
            mockCardRepository.Verify(x => x.Update(It.IsAny<Card>()), Times.Never);
            mockCardTransactionRepository.Verify(x => x.Insert(It.IsAny<CardTransaction>()), Times.Never);
            mockIAuthenticationManager.Verify(x => x.IsAuthenticated(), Times.Once);
            Assert.False(status);
        }

    }
}
