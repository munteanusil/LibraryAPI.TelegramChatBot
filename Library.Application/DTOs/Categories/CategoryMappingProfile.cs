using AutoMapper;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.Categories
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {

            CreateMap<Category, CategoryDto>()
               .ReverseMap()
               .ForMember(dest => dest.Id, opt => opt.Ignore()); 

            CreateMap<CreateCategoryDto, Category>()
                .ReverseMap();

         
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Books, opt => opt.Ignore());
        }
    }
}
