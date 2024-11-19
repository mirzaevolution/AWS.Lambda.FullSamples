using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using Customers.Lambda.Annotations.Models;

namespace Customers.Lambda.Annotations.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly IDynamoDBContext _dynamoDBContext;
		private readonly IMapper _mapper;

		public CustomerService(
				IAmazonDynamoDB amazonDynamoDBClient,
				IMapper mapper
			)
		{
			_dynamoDBContext = new DynamoDBContext(amazonDynamoDBClient);
			_mapper = mapper;
		}

		public async Task<IEnumerable<CustomerDto>> GetByCountry(string country)
		{
			if (string.IsNullOrEmpty(country))
			{
				throw new ArgumentNullException(nameof(country));
			}
			List<Customer> customers =
				await _dynamoDBContext.QueryAsync<Customer>(country)
				.GetRemainingAsync();

			IEnumerable<CustomerDto> result =
				customers == null ? Enumerable.Empty<CustomerDto>() :
				_mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerDto>>(customers);
			return result;
		}

		public async Task<CustomerDto?> GetByCountryAndEmail(string country, string email)
		{
			if (string.IsNullOrEmpty(country))
			{
				throw new ArgumentNullException(nameof(country));
			}
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException(nameof(email));
			}
			Customer customer = await _dynamoDBContext.LoadAsync<Customer>(country, email);
			return customer == null ? null : _mapper.Map<Customer, CustomerDto>(customer);

		}

		public async Task<CustomerDto> CreateOrUpdate(CustomerDto dto)
		{
			if (dto == null)
			{
				throw new ArgumentNullException(nameof(dto));
			}
			Customer customer = _mapper.Map<CustomerDto, Customer>(dto);
			await _dynamoDBContext.SaveAsync<Customer>(customer, new DynamoDBOperationConfig
			{
				ConditionalOperator = Amazon.DynamoDBv2.DocumentModel.ConditionalOperatorValues.And
			});
			return _mapper.Map<CustomerDto>(customer);
		}

		public async Task Delete(string country, string email)
		{
			if (string.IsNullOrEmpty(country))
			{
				throw new ArgumentNullException(nameof(country));
			}
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException(nameof(email));
			}
			await _dynamoDBContext.DeleteAsync<Customer>(country, email);
		}
	}
}
