using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualCashCard.Repository.Interfaces
{
    public interface IRepository<T>
    {
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        T Get();
    }
}
