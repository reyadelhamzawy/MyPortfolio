using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object Id);
        void Insert(T Entity);
        void Update(T Entity);
        void Delete(object Id);
    }
}
