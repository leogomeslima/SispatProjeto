using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;

namespace Sispat.API.Controllers
{
    // Vamos proteger Categorias e Localizações também.
    [Authorize]
    public class CategoriesController : BaseApiController
    {
        private readonly IGenericRepository<Category> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repo = unitOfWork.Categories;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDtos>>> GetAll()
        {
            var categories = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDtos>>(categories));
        }

        // --- NOVO ---
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDtos>> GetById(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(_mapper.Map<CategoryDtos>(category));
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDtos>> Create([FromBody] CreateOrUpdateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _repo.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, _mapper.Map<CategoryDtos>(category));
        }

        // --- NOVO ---
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrUpdateCategoryDto dto)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return NotFound();

            _mapper.Map(dto, category); // Mapeia o DTO para a entidade existente
            _repo.Update(category);
            await _unitOfWork.CompleteAsync();

            return NoContent(); // 204 No Content
        }

        // --- NOVO ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return NotFound();

            _repo.Delete(category);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception) // Provavelmente DbUpdateException por causa da FK
            {
                // Retorna 400 Bad Request se a categoria estiver em uso
                return BadRequest(new { message = "Não é possível excluir esta categoria, pois ela está sendo usada por ativos." });
            }

            return NoContent(); // 204 No Content
        }
    }
}