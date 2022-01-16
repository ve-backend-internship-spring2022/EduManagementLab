using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        IEnumerable<T> GetAll();
        T? GetById(Guid id);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);

    }
}
