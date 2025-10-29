using AutoMapper;
using Sispat.Application.DTOs;
using Sispat.Application.Interfaces;
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AssetService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AssetDto> CreateAssetAsync(CreateAssetDto createDto)
        {
            // Mapeia o DTO (entrada) para a Entidade (domínio)
            var asset = _mapper.Map<Asset>(createDto);

            // Lógica de negócio (ex: garantir que o status inicial seja 'EmEstoque' se não alocado)
            if (string.IsNullOrEmpty(asset.AssignedToUserId))
            {
                asset.Status = Domain.Enums.AssetStatus.EmEstoque;
            }

            await _unitOfWork.Assets.AddAsync(asset);
            await _unitOfWork.CompleteAsync();

            // Mapeia a Entidade (completa, com ID) de volta para o DTO (saída)          
            var createdAsset = await _unitOfWork.Assets.GetByIdAsync(asset.Id);
            return _mapper.Map<AssetDto>(createdAsset);
        }

        public async Task<bool> DeleteAssetAsync(Guid id)
        {
            var asset = await _unitOfWork.Assets.GetByIdAsync(id);
            if (asset == null)
            {
                return false;
            }

            _unitOfWork.Assets.Delete(asset);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<AssetDto>> GetAllAssetsAsync()
        {
            var assets = await _unitOfWork.Assets.GetAllAsync();
            // Mapeia a lista de Entidades para uma lista de DTOs
            return _mapper.Map<IEnumerable<AssetDto>>(assets);
        }

        public async Task<AssetDto?> GetAssetByIdAsync(Guid id)
        {
            var asset = await _unitOfWork.Assets.GetByIdAsync(id);
            if (asset == null)
            {
                return null;
            }
            return _mapper.Map<AssetDto>(asset);
        }

        public async Task<bool> UpdateAssetAsync(UpdateAssetDto updateDto)
        {
            var asset = await _unitOfWork.Assets.GetByIdAsync(updateDto.Id);
            if (asset == null)
            {
                return false; 
            }

            // Mapeia os dados do DTO para a entidade existente (rastreada pelo EF Core)
            _mapper.Map(updateDto, asset);

            //_unitOfWork.Assets.Update(asset); // O EF Core já está rastreando, basta salvar.
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
