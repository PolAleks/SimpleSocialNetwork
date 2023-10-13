using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SimpleSocialNetwork.DAL.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetwork.DAL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _db;

        public DbSet<TEntity> Set
        {
            get;
            private set;
        }

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            Set = _db.Set<TEntity>();
        }

        public async Task Create(TEntity item)
        {
            await Set.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(TEntity item)
        {
            Set.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<TEntity> Get(int id)
        {
            return await Set.FindAsync(id);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await Set.ToListAsync();
        }

        public async Task Update(TEntity item)
        {
            Set.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
