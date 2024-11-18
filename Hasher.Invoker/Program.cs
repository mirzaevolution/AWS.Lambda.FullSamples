using Amazon.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hasher.Invoker
{
	internal class Program
	{
		private static IConfiguration? _configuration;
		private static IServiceProvider? _serviceProvider;
		static void Configure()
		{
			_configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var services = new ServiceCollection();
			services.AddSingleton<IConfiguration>(_configuration);
			services.AddAWSService<IAmazonLambda>(_configuration.GetAWSOptions());
			services.AddSingleton<IHasherInvokerService, HasherInvokerService>();
			_serviceProvider = services.BuildServiceProvider();
		}


		static async Task Samples()
		{
			IHasherInvokerService hasherInvokerService =
				_serviceProvider!.GetRequiredService<IHasherInvokerService>();
			string textToHash = "Hello world!";
			string hashedResult = await hasherInvokerService.GetHash(textToHash);
			Console.WriteLine($"Text to hash: {textToHash}");
			Console.WriteLine($"Hash result: {hashedResult}");
		}

		static void Main(string[] args)
		{
			Configure();
			Samples().Wait();
			Console.ReadLine();
		}
	}
}
