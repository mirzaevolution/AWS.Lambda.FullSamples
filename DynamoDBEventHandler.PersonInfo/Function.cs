using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoDBEventHandler.PersonInfo;

public class Function
{
	public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
	{
		context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

		foreach (var record in dynamoEvent.Records)
		{
			context.Logger.LogInformation($"Event ID: {record.EventID}");
			context.Logger.LogInformation($"Event Name: {record.EventName}");
			if (record.Dynamodb != null)
			{
				context.Logger.LogInformation(JsonSerializer.Serialize(record.Dynamodb.NewImage));
			}
		}

		context.Logger.LogInformation("Stream processing complete.");
	}
}