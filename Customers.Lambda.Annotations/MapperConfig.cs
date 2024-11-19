using AutoMapper;
using Customers.Lambda.Annotations.Models;
namespace Customers.Lambda.Annotations
{
	public class MapperConfig : Profile
	{
		public MapperConfig()
		{
			CreateMap<CustomerDto, Customer>().ReverseMap();
		}
	}
}
