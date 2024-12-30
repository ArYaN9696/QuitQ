using AutoMapper;
using quitq_cf.DTO;
using quitq_cf.Models;
namespace quitq_cf.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, RegisterCustomerDTO>().ReverseMap();
            CreateMap<Admin, RegisterAdminDTO>().ReverseMap();
            CreateMap<Seller, RegisterSellerDTO>().ReverseMap();

            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<CreateOrderDTO, Order>();
            CreateMap<CartItemDTO, Cart>().ReverseMap();
            CreateMap<CreateProductDTO, Product>().ReverseMap();
            CreateMap<OrderDetailDTO, OrderDetail>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
            CreateMap<ProcessPaymentDTO, Payment>().ReverseMap();
        }

    }
}
