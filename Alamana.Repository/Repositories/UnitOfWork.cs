using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;

namespace Alamana.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AlamanaBbContext _context;
        private Hashtable _repositoies;
        public UnitOfWork(AlamanaBbContext context)
        {
            _context = context;
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositoies == null)
            {
                _repositoies = new Hashtable();
            }

            var entityKey = typeof(TEntity).Name;
            if (!_repositoies.ContainsKey(entityKey))
            {
                var repositoryInstane = Activator.CreateInstance(typeof(GenericRepository<>).MakeGenericType(typeof(TEntity)), _context);
                _repositoies.Add(entityKey, repositoryInstane);
            }

            return (IGenericRepository<TEntity>)_repositoies[entityKey];
        }
    }
}
