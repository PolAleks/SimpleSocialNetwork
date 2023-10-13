using Microsoft.EntityFrameworkCore;
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
            var set = _db.Set<TEntity>();
            set.Load();

            Set = set;
        }

        public void Create(TEntity item)
        {
            Set.Add(item);
            _db.SaveChanges();
        }

        public void Delete(TEntity item)
        {
            Set.Remove(item);
            _db.SaveChanges();
        }

        public TEntity Get(int id)
        {
            return Set.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Set;
        }

        public void Update(TEntity item)
        {
            Set.Update(item);
            _db.SaveChanges();
        }
    }
}
