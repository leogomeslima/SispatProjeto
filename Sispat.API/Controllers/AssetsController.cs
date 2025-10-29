using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sispat.Application.DTOs;
using Sispat.Application.Interfaces;

namespace Sispat.API.Controllers
{
    [Authorize] // Protege este controlador! Só acessa quem está logado.
    public class AssetsController : BaseApiController
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAllAssets()
        {
            var assets = await _assetService.GetAllAssetsAsync();
            return Ok(assets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssetDto>> GetAssetById(Guid id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            return Ok(asset);
        }

        [HttpPost]
        public async Task<ActionResult<AssetDto>> CreateAsset([FromBody] CreateAssetDto createDto)
        {
            // O FluentValidation é executado automaticamente pelo 'AddFluentValidationAutoValidation()'
            // Se o DTO for inválido, a API retornará 400 Bad Request automaticamente.

            var newAsset = await _assetService.CreateAssetAsync(createDto);
            // Retorna 201 Created com a rota para o novo recurso
            return CreatedAtAction(nameof(GetAssetById), new { id = newAsset.Id }, newAsset);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsset([FromBody] UpdateAssetDto updateDto)
        {
            var result = await _assetService.UpdateAssetAsync(updateDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent(); // 204 No Content (Sucesso, sem corpo de resposta)
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(Guid id)
        {
            var result = await _assetService.DeleteAssetAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent(); // 204 No Content
        }
    }
}
