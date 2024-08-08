using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.Models;

namespace UrlShorten.DataAccess.Repository.IRepository
{
    public interface IRepository<TEntity,TKey> where TEntity : class, IEntity<TKey> where TKey : IComparable
    {
        Task AddAsync(TEntity entity);
        Task<IList<TEntity>> GetAllAsync();
        Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<TEntity> GetByIdAsync(TKey id);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null);
        void Remove(TEntity entityToDelete);
        void Remove(TKey id);
        void Remove(Expression<Func<TEntity, bool>> filter);
    }
}
