using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Customers.Lambda.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Customers.Lambda;

public class Function
{
	private readonly IDynamoDBContext _dynamoDBContext;

	public Function()
	{
		AmazonDynamoDBClient amazonDynamoDBClient = new AmazonDynamoDBClient();
		_dynamoDBContext = new DynamoDBContext(amazonDynamoDBClient);
	}

	public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
	{
		string path = request.RequestContext.Http.Path.ToLower() ?? string.Empty;
		if (string.IsNullOrEmpty(path))
		{
			return StatusResponse<string>("Not found", 404);
		}
		string method = request.RequestContext.Http.Method?.ToLower() ?? string.Empty;
		if (string.IsNullOrEmpty(method))
		{
			return StatusResponse<string>("Invalid request method", 400);
		}

		if (Regex.IsMatch(path, @"/api/customers/\w+/[\w-\.]+@([\w-]+\.)+[\w-]{2,4}(/?)", RegexOptions.IgnoreCase | RegexOptions.Compiled) && method == "get")
		{
			return await GetCustomer(request, context);
		}
		if (Regex.IsMatch(path, @"/api/customers/\w+(/?)", RegexOptions.IgnoreCase | RegexOptions.Compiled) && method == "get")
		{
			return await GetCustomers(request, context);
		}
		else if (path.StartsWith("/api/customers") && method == "post")
		{
			return await PostCustomer(request, context);
		}
		return StatusResponse<string>("Invalid request.", 400);
	}


	private async Task<APIGatewayHttpApiV2ProxyResponse> GetCustomer(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
	{
		context.Logger.LogInformation($"Processing [{nameof(GetCustomer)}] request");

		if (request.PathParameters.TryGetValue("country", out string? country) && !string.IsNullOrEmpty(country) &&
			request.PathParameters.TryGetValue("email", out string? email) && !string.IsNullOrEmpty(email))
		{
			Customer customer = await _dynamoDBContext.LoadAsync<Customer>(country, email);
			if (customer != null)
			{
				context.Logger.LogInformation("Returning response:");
				context.Logger.LogInformation(JsonSerializer.Serialize(customer, new JsonSerializerOptions { WriteIndented = true }));
				return StatusResponse<Customer>(customer, 200);
			}
		}
		return StatusResponse<string>("Customer doesn't exist", 404);
	}

	private async Task<APIGatewayHttpApiV2ProxyResponse> GetCustomers(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
	{
		context.Logger.LogInformation($"Processing [{nameof(GetCustomers)}] request");

		if (request.PathParameters.TryGetValue("country", out string? country) && !string.IsNullOrEmpty(country))
		{
			List<Customer> customers = await _dynamoDBContext.QueryAsync<Customer>(country).GetRemainingAsync();
			return StatusResponse<List<Customer>>(customers, 200);
		}
		return StatusResponse<string>("Specified country returns empty record", 404);
	}


	private async Task<APIGatewayHttpApiV2ProxyResponse> PostCustomer(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
	{
		context.Logger.LogInformation($"Processing [{nameof(PostCustomer)}] request");
		if (!string.IsNullOrEmpty(request.Body))
		{
			Customer? customer = null;

			try
			{
				customer = JsonSerializer.Deserialize<Customer>(request.Body);
			}
			catch (Exception ex)
			{
				context.Logger.LogError($"Invalid deserializing body of:");
				context.Logger.LogError(request.Body);
				context.Logger.LogError(ex.ToString());
			}

			if (customer != null)
			{
				if (customer.Id == Guid.Empty.ToString())
				{
					customer.Id = Guid.NewGuid().ToString();
				}
				await _dynamoDBContext.SaveAsync<Customer>(customer);
				return StatusResponse<Customer>(customer, 200);
			}

		}
		return StatusResponse<string>("Invalid body request", 400);
	}
	private APIGatewayHttpApiV2ProxyResponse StatusResponse<T>(T body, int statusCode) => new APIGatewayHttpApiV2ProxyResponse
	{
		Body = JsonSerializer.Serialize(body),
		StatusCode = statusCode,
		Headers = new Dictionary<string, string>
		{
			{ "Content-Type", "application/json" }
		}
	};






}
