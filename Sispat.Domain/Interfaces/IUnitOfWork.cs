using Sispat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAssetRepository Assets { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Location> Locations { get; }
        Task<int> SaveChangesAsync();
    }
}
