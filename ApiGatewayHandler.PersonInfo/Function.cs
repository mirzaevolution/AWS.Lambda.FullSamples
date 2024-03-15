using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using PersonInfo.Shared;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ApiGatewayHandler.PersonInfo;

public class Function
{
	private readonly IDynamoDBContext _dynamoDBContext;

	public Function()
	{
		var dynamoDBClient = new AmazonDynamoDBClient();
		_dynamoDBContext = new DynamoDBContext(dynamoDBClient);
	}

	public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
		APIGatewayHttpApiV2ProxyRequest request,
		ILambdaContext context)
	{
		var method = request.RequestContext.Http.Method.ToUpper();
		switch (method)
		{
			case "GET":
				{
					return await GetPerson(request, context);
				}
			case "POST":
				{
					return await PostPerson(request, context);
				}
		}

		return StatusResponse<string>("Invalid request", 400);
	}

	private APIGatewayHttpApiV2ProxyResponse StatusResponse<T>(T body, int statusCode) =>
		new APIGatewayHttpApiV2ProxyResponse
		{
			Body = JsonSerializer.Serialize(body),
			StatusCode = statusCode
		};

	#region Function Handlers
	public async Task<APIGatewayHttpApiV2ProxyResponse> GetPerson
		(APIGatewayHttpApiV2ProxyRequest request,
		ILambdaContext context)
	{
		string errorMessage = "Invalid request";
		int statusCode = 400;
		if (request.PathParameters.TryGetValue("id", out string? idString) && Guid.TryParse(idString, out Guid id))
		{
			try
			{
				var personInfo = await _dynamoDBContext.LoadAsync<PersonInfoModel>(id);
				if (personInfo != null)
				{
					context.Logger.LogInformation("Data found:");
					context.Logger.LogInformation(JsonSerializer.Serialize(personInfo));
					statusCode = 200;
					return StatusResponse<PersonInfoModel>(personInfo, statusCode);
				}
				else
				{
					errorMessage = $"Person info with id: {id} not found";
					statusCode = 404;
				}
			}
			catch (Exception ex)
			{
				context.Logger.LogError(ex.ToString());
				errorMessage = ex.Message;
			}
		}

		context.Logger.LogError(errorMessage);
		return StatusResponse<string>(errorMessage, statusCode);
	}

	public async Task<APIGatewayHttpApiV2ProxyResponse> PostPerson(
		APIGatewayHttpApiV2ProxyRequest request,
		ILambdaContext context)
	{
		string errorMessage = "Invalid request";
		int statusCode = 400;
		if (!string.IsNullOrEmpty(request.Body))
		{
			try
			{
				PersonInfoModel? personInfo = JsonSerializer.Deserialize<PersonInfoModel>(request.Body);
				if (personInfo != null)
				{
					await _dynamoDBContext.SaveAsync<PersonInfoModel>(personInfo);
					context.Logger.LogInformation("Data saved to db.");
					context.Logger.LogInformation(JsonSerializer.Serialize<PersonInfoModel>(personInfo));
					statusCode = 200;
					return StatusResponse<PersonInfoModel>(personInfo, statusCode);
				}
			}
			catch (Exception ex)
			{
				context.Logger.LogError(ex.ToString());

			}
			errorMessage = "Failed to deserialize the request body";
		}
		context.Logger.LogError(errorMessage);
		return StatusResponse<string>(errorMessage, statusCode);

	}
	#endregion
}
