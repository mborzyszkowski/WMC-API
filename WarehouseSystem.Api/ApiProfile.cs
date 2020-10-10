using System.Linq;
using AutoMapper;
using WarehouseSystem.Core.Entity;
using WarehouseSystem.Query;

namespace WarehouseSystem
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<Product, ProductResult>()
                .ForMember(
                    p => p.Quantity,
                    opt => opt.MapFrom(p => p.QuantityChanges.Sum(q => q.Quantity)));
        }
    }
}