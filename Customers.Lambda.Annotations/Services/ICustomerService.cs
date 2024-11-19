using Customers.Lambda.Annotations.Models;

namespace Customers.Lambda.Annotations.Services
{
	public interface ICustomerService
	{
		Task<CustomerDto> CreateOrUpdate(CustomerDto dto);
		Task<CustomerDto?> GetByCountryAndEmail(string country, string email);
		Task<IEnumerable<CustomerDto>> GetByCountry(string country);
		Task Delete(string country, string email);
	}
}
