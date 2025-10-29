using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;

namespace Sispat.API.Controllers
{
    // Vamos deixar Categorias e Localizações públicas (sem [Authorize])
    // para facilitar a criação e listagem no front-end.
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

        [HttpPost]
        public async Task<ActionResult<CategoryDtos>> Create([FromBody] CreateOrUpdateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _repo.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, _mapper.Map<CategoryDtos>(category));
        }
    }
}