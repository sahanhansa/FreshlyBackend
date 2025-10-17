using AutoMapper;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.PlacedDate, opt => opt.MapFrom(src => src.PlacedAt.HasValue ? src.PlacedAt.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(dest => dest.PlacedTime, opt => opt.MapFrom(src => src.PlacedAt.HasValue ? src.PlacedAt.Value.ToString("HH:mm:ss") : null));
        }
    }
}