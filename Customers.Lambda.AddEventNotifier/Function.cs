using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Customers.Lambda.Events;

public class Function
{
	private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;

	public Function()
	{
		_amazonSimpleNotificationService = new AmazonSimpleNotificationServiceClient();
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
		context.Logger.LogInformation($"Processed message:");
		context.Logger.LogInformation(message.Body);
		await _amazonSimpleNotificationService.PublishAsync(new PublishRequest
		{
			TopicArn = Environment.GetEnvironmentVariable("CustomersTopic"),
			Subject = "Customers.Lambda.Events",
			Message = message.Body
		});
	}
}