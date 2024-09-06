using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.DataAccess.Context;
using UrlShorten.DataAccess.Repository.IRepository;
using UrlShorten.Models;


namespace UrlShorten.DataAccess.Repository
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : IComparable
    {
        private readonly UrlShortenDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(UrlShortenDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            IQueryable<TEntity> query = _dbSet;
            return await query.ToListAsync();
        }

        public async Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if(filter is not null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if(filter is not null)
                query = query.Where(filter);
            return await query.CountAsync();
        }

        public void Remove(TEntity entityToDelete)
        {
            _dbSet.Remove(entityToDelete);
        }

        public void Remove(TKey id)
        {
            var entityToDelete = _dbSet.Find(id);
            Remove(entityToDelete);
        }

        public void Remove(Expression<Func<TEntity, bool>> filter)
        {
            _dbSet.RemoveRange(_dbSet.Where(filter));
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _dbSet.SingleOrDefaultAsync(filter);
            }

            return null; // Or throw an exception if filter is required
        }
    }
}
