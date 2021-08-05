using AutoMapper;
using CarDealer.DataTransferObjects.InputModels;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<CustomerInputModel,Customer>();
            this.CreateMap<SaleInputModel, Sale>();
        }
    }
}
