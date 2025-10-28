using Sispat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Domain.Interfaces
{
    public interface IAssetRepository : IGenericRepository<Asset>
    {
        // implementar depois métodos específicos para Asset, se necessário
    }
}
