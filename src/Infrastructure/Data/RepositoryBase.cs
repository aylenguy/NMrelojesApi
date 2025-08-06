using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{//Clase genérica, repositorio base, T es como un comodin que nos permite usarlo en todas las entidades. 
    //Esta clase no existe hasta el momento en que se la llama a traves de otro repositorio. 
    public class RepositoryBase<T> where T : class
    {
        // Contexto de base de datos de Entity Framework
        private readonly DbContext _context;
        public RepositoryBase(ApplicationContext context)
        {
            _context = context;
        }

        public T? Get<TId>(TId id)
        {
            return _context.Set<T>().Find(new object[] { id });
        }

        public List<T> Get()
        {
            return _context.Set<T>().ToList();
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}

