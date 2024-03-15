using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using PersonInfo.Shared;
using System.Text.Json;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SqsHandler.PersonInfo;
public class Function
{
	private readonly IAmazonDynamoDB _amazonDynamoDB;
	private readonly IDynamoDBContext _amazonDynamoDBcontext;
	public Function()
	{
		_amazonDynamoDB = new AmazonDynamoDBClient();
		_amazonDynamoDBcontext = new DynamoDBContext(_amazonDynamoDB);
	}

	public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
	{
		foreach (var message in evnt.Records)
		{
			await ProcessMessageAsync(message, context);
		}
	}

	private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
	{
		if (message != null && !string.IsNullOrEmpty(message.Body))
		{
			context.Logger.LogInformation($"Processed message {message.Body}");

			PersonInfoModel? personInfoModel = JsonSerializer.Deserialize<PersonInfoModel>(message.Body);
			if (personInfoModel != null)
			{
				await _amazonDynamoDBcontext.SaveAsync<PersonInfoModel>(personInfoModel);
				context.Logger.LogInformation("PersonInfo object saved to dynamo-db table");

			}
			else
			{
				context.Logger.LogInformation("Deserialized object is null. Failed to process");

			}
		}
		else
		{
			context.Logger.LogInformation("Message body is null. Failed to process");
		}


		await Task.CompletedTask;
	}
}