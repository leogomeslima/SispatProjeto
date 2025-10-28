using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;
using Sispat.Infrastructure.Persitence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Repositórios privados
        private IAssetRepository? _assetRepository;
        private IGenericRepository<Category>? _categoryRepository;
        private IGenericRepository<Location>? _locationRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Repositórios públicos (com inicialização preguiçosa/lazy loading)
        public IAssetRepository Assets =>
            _assetRepository ??= new AssetRepository(_context);

        public IGenericRepository<Category> Categories =>
            _categoryRepository ??= new GenericRepository<Category>(_context);

        public IGenericRepository<Location> Locations =>
            _locationRepository ??= new GenericRepository<Location>(_context);


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
