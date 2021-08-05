using AutoMapper;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputDto, User>();
            this.CreateMap<ProductInputDto, Product>();
            this.CreateMap<CategoriesInputDto, Category>();
            this.CreateMap<CategoriesProductsDto, CategoryProduct>();
        }
    }
}
