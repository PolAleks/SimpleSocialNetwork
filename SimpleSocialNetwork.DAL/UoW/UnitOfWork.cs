using Microsoft.EntityFrameworkCore.Infrastructure;
using SimpleSocialNetwork.DAL.Db;
using SimpleSocialNetwork.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetwork.DAL.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _appContext;

        private Dictionary<Type, object> _repositories;

        public UnitOfWork(ApplicationDbContext app)
        {
            _appContext = app;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            _repositories ??= new Dictionary<Type, object>();

            if (hasCustomRepository)
            {
                var customRepo = _appContext.GetService<IRepository<TEntity>>();
                if (customRepo != null)
                {
                    return customRepo;
                }
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(_appContext);
            }

            return (IRepository<TEntity>)_repositories[type];

        }
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            throw new NotImplementedException();
        }

    }
}
