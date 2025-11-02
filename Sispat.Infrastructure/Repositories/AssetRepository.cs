using Microsoft.EntityFrameworkCore; // <-- IMPORTE O ENTITYFRAMEWORKCORE
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;

using Sispat.Infrastructure.Persitence;

namespace Sispat.Infrastructure.Repositories
{
    public class AssetRepository : GenericRepository<Asset>, IAssetRepository
    {
        public AssetRepository(AppDbContext context) : base(context)
        {
        }

        // --- NOVO MÉTODO (Sobrescrito) ---
        // Agora, ao buscar todos os ativos, ele "Inclui" os dados relacionados
        public override async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Include(a => a.AssignedToUser)
                .AsNoTracking() // Opcional, mas bom para performance em listas
                .ToListAsync();
        }

        // --- NOVO MÉTODO (Sobrescrito) ---
        // O FindAsync() (do genérico) NÃO suporta .Include()
        // Precisamos sobrescrever o GetByIdAsync para usar .FirstOrDefaultAsync()
        public override async Task<Asset?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Include(a => a.AssignedToUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}