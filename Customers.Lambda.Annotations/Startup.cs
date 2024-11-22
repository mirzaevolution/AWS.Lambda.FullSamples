using Amazon.DynamoDBv2;
using Amazon.SQS;
using Customers.Lambda.Annotations.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Lambda.Annotations;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{

	public void ConfigureServices(IServiceCollection services)
	{


		var configuration = new ConfigurationBuilder()
							.AddJsonFile("appsettings.json", true, true)
							.Build();

		services.AddSingleton<IConfiguration>(configuration);
		services.AddDefaultAWSOptions(configuration.GetAWSOptions());

		services.AddAWSService<IAmazonDynamoDB>();
		services.AddAWSService<IAmazonSQS>();

		services.AddAutoMapper(typeof(Startup));
		services.AddSingleton<ICustomerService, CustomerService>();

		//// Add AWS Systems Manager as a potential provider for the configuration. This is 
		//// available with the Amazon.Extensions.Configuration.SystemsManager NuGet package.
		//builder.AddSystemsManager("/app/settings");


		//// Example of using the AWSSDK.Extensions.NETCore.Setup NuGet package to add
		//// the Amazon S3 service client to the dependency injection container.
		//services.AddAWSService<Amazon.S3.IAmazonS3>();
	}
}
