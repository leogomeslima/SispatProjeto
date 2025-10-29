using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;

namespace Sispat.API.Controllers
{
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

        [HttpPost]
        public async Task<ActionResult<LocationDto>> Create([FromBody] CreateOrUpdateLocationDto dto)
        {
            var location = _mapper.Map<Location>(dto);
            await _repo.AddAsync(location);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetAll), new { id = location.Id }, _mapper.Map<LocationDto>(location));
        }
    }
}
