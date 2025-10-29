using AutoMapper;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamentos de Categoria
            CreateMap<Category, CategoryDtos>();
            CreateMap<CreateOrUpdateCategoryDto, Category>();

            // Mapeamentos de Localização
            CreateMap<Location, LocationDto>();
            CreateMap<CreateOrUpdateLocationDto, Location>();

            // Mapeamentos de Ativo (Asset)
            CreateMap<CreateAssetDto, Asset>();
            CreateMap<UpdateAssetDto, Asset>();

            // Mapeamento de Leitura (Asset -> AssetDto)
            CreateMap<Asset, AssetDto>()
                // Mapeia o Enum para String
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()))

                // Mapeia propriedades aninhadas
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category.Name))

                .ForMember(dest => dest.LocationName,
                           opt => opt.MapFrom(src => src.Location != null ? src.Location.Name : null))

                .ForMember(dest => dest.AssignedToUserName,
                           opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : null));
        }
    }
}
