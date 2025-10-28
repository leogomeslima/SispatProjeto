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
    public class AssetRepository : GenericRepository<Asset>, IAssetRepository
    {
        public AssetRepository(AppDbContext context) : base(context)
        {
        }

        // Exemplo de como você implementaria um método customizado
        // public async Task<IEnumerable<Asset>> GetAssetsByStatusAsync(AssetStatus status)
        // {
        //     return await _dbSet.Where(a => a.Status == status).ToListAsync();
        // }
    }
}
