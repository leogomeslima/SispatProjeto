using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;

namespace Sispat.API.Controllers
{
    [Authorize]
    public class LocationsController : BaseApiController
    {
        private readonly IGenericRepository<Location> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repo = unitOfWork.Locations;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
        {
            var locations = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<LocationDto>>(locations));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetById(Guid id)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) return NotFound();
            return Ok(_mapper.Map<LocationDto>(location));
        }

        [HttpPost]
        public async Task<ActionResult<LocationDto>> Create([FromBody] CreateOrUpdateLocationDto dto)
        {
            var location = _mapper.Map<Location>(dto);
            await _repo.AddAsync(location);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = location.Id }, _mapper.Map<LocationDto>(location));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrUpdateLocationDto dto)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) return NotFound();

            _mapper.Map(dto, location);
            _repo.Update(location);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) return NotFound();

            _repo.Delete(location);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não é possível excluir esta localização, pois ela está sendo usada por ativos." });
            }

            return NoContent();
        }
    }
}