using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Repositories.Interfaces;
using Domain.Entities;

namespace DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }

        Task<int> CompleteAsync();
    }
}
