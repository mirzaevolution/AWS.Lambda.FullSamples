using Amazon.Lambda.Core;
using System.Security.Cryptography;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Hasher.Lambda;

public class Function
{


	public string FunctionHandler(string input, ILambdaContext context)
	{
		if (string.IsNullOrEmpty(input))
		{
			context.Logger.LogError("Cannot proceed the request to due empty payload.");
			throw new ArgumentNullException(nameof(input));
		}

		context.Logger.LogInformation($"AppCreator: {Environment.GetEnvironmentVariable("AppCreator") ?? "-"}");
		context.Logger.LogInformation($"AppCompany: {Environment.GetEnvironmentVariable("AppCompany") ?? "-"}");
		context.Logger.LogInformation($"AppEmail: {Environment.GetEnvironmentVariable("AppEmail") ?? "-"}");

		using SHA512 sha512 = SHA512.Create();
		byte[] inputBytes = Encoding.UTF8.GetBytes(input);
		string encodedString = Convert.ToBase64String(
				sha512.ComputeHash(inputBytes)
			);
		context.Logger.LogInformation($"Hash input: {input}");
		context.Logger.LogInformation($"Hash result: {encodedString}");
		return encodedString;
	}
}
