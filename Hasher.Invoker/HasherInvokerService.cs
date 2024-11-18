using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Hasher.Invoker
{
	public interface IHasherInvokerService
	{
		Task<string> GetHash(string input);
	}
	public class HasherInvokerService : IHasherInvokerService
	{
		private readonly IAmazonLambda _client;
		private readonly IConfiguration _configuration;
		public HasherInvokerService
				(
					IAmazonLambda client,
					IConfiguration configuration
			)
		{
			_client = client;
			_configuration = configuration;
		}

		public async Task<string> GetHash(string input)
		{
			string functionName = _configuration["Functions:Hasher"] ??
				throw new NullReferenceException("Function name is empty");
			InvokeRequest invokeRequest = new InvokeRequest
			{
				FunctionName = functionName,
				Payload = JsonSerializer.Serialize(input)
			};
			InvokeResponse response = await _client.InvokeAsync(invokeRequest);
			return Encoding.UTF8.GetString(response.Payload.ToArray());
		}
	}
}
