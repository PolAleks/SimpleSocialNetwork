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
        public DbContext _db;

        public DbSet<TEntity> Set
        {
            get;
            set;
        }

        public Repository(DbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            Set = db.Set<TEntity>();
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
