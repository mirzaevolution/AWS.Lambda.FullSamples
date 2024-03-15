using Amazon.Lambda.Core;
using Hello.Lambda.Models;
using System.Text;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Hello.Lambda;

public class Function
{
	public Task<InvokeResponse> FunctionHandler(object eventData, ILambdaContext context)
	{
		InvokeRequest request = new InvokeRequest();
		if (eventData != null)
		{
			context.Logger.Log("Raw object request: ");
			context.Logger.Log(eventData.ToString());
			ExtractedBody? payload = JsonSerializer.Deserialize<ExtractedBody>(eventData.ToString());
			if (payload != null && payload.Body != null)
			{
				request = JsonSerializer.Deserialize<InvokeRequest>(payload.Body.ToString()) ?? new InvokeRequest();
			}
		}
		var response = new InvokeResponse
		{
			OriginalMessage = request != null && !string.IsNullOrEmpty(request.Message) ? request.Message : "-",
			UppercasedMessage = request != null && !string.IsNullOrEmpty(request.Message) ? request.Message.ToUpper() : "-",
			LowercasedMessage = request != null && !string.IsNullOrEmpty(request.Message) ? request.Message.ToLower() : "-",
			Base64EncodedMessage = request != null && !string.IsNullOrEmpty(request.Message) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Message)) : "-"
		};

		context.Logger.Log($"Processing original message: `{response.OriginalMessage}`");
		context.Logger.Log($"Response: `${JsonSerializer.Serialize(response)}`");
		return Task.FromResult(response);

	}
}
