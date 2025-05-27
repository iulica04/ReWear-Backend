using Application.DTOs;
using Application.Use_Cases.Commands;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Application.Use_Cases.Commands.OutfitCommands;
using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Application.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<CreateUserCommand, User>().ReverseMap();
            CreateMap<UpdateUserCommand, User>().ReverseMap();

            
            CreateMap<CreateClothingItemCommand, ClothingItem>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => new ClothingTag { Tag = tag }).ToList()));
            CreateMap<UpdateClothingItemCommand, ClothingItem>().ReverseMap();
            CreateMap<ClothingItem, ClothingItemDTO>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Tag).ToList()));

            CreateMap<Outfit, OutfitDTO>().ReverseMap();
            CreateMap<UpdateOutfitCommand, Outfit>().ReverseMap();

            CreateMap<FavoriteOutfit, FavoriteOutfitDTO>().ReverseMap();
        }
    }
}
