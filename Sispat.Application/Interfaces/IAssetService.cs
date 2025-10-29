using Sispat.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.Interfaces
{
    public interface IAssetService
    {
        Task<AssetDto?> GetAssetByIdAsync(Guid id);
        Task<IEnumerable<AssetDto>> GetAllAssetsAsync();
        Task<AssetDto> CreateAssetAsync(CreateAssetDto createDto);
        Task<bool> UpdateAssetAsync(UpdateAssetDto updateDto);
        Task<bool> DeleteAssetAsync(Guid id);
    }
}
