using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Customers.Lambda.Annotations.Models;
using Customers.Lambda.Annotations.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Customers.Lambda.Annotations;


public class Functions(ICustomerService _customerService)
{
	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Get, "/v1/customers/{country}/{email}")]
	public async Task<IHttpResult> GetByCountryAndEmail(string country, string email, ILambdaContext context)
	{
		context.Logger.LogInformation($"[{nameof(GetByCountryAndEmail)}] - Country: {country}, Email: {email}");
		if (string.IsNullOrEmpty(country))
		{
			return HttpResults.BadRequest(new
			{
				isError = true,
				errors = new[] { "Invalid country" }
			});
		}
		if (string.IsNullOrEmpty(email))
		{
			return HttpResults.BadRequest(new
			{
				isError = true,
				errors = new[] { "Invalid email" }
			});
		}
		try
		{
			var result = await _customerService.GetByCountryAndEmail(country, email);
			context.Logger.LogInformation($"[{nameof(GetByCountryAndEmail)}] - Result: {JsonSerializer.Serialize(result)}");

			return HttpResults.Ok(new
			{
				isError = false,
				errors = Enumerable.Empty<string>(),
				data = result
			});
		}
		catch (Exception ex)
		{
			context.Logger.LogError($"[{nameof(GetByCountryAndEmail)}] - Error");
			context.Logger.LogError($"[{nameof(GetByCountryAndEmail)}] - {ex.Message}");
			return HttpResults.InternalServerError(new
			{
				isError = true,
				errors = new[] { ex.Message }
			});
		}
	}


	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Get, "/v1/customers/{country}")]
	public async Task<IHttpResult> GetByCountry(string country, ILambdaContext context)
	{
		context.Logger.LogInformation($"[{nameof(GetByCountry)}] - Country: {country}");
		if (string.IsNullOrEmpty(country))
		{
			return HttpResults.BadRequest(new
			{
				isError = true,
				errors = new[] { "Invalid country" }
			});
		}
		try
		{
			var result = await _customerService.GetByCountry(country);
			context.Logger.LogInformation($"[{nameof(GetByCountry)}] - Result: {JsonSerializer.Serialize(result)}");

			return HttpResults.Ok(new
			{
				isError = false,
				errors = Enumerable.Empty<string>(),
				data = result
			});
		}
		catch (Exception ex)
		{
			context.Logger.LogError($"[{nameof(GetByCountry)}] - Error");
			context.Logger.LogError($"[{nameof(GetByCountry)}] - {ex.Message}");
			return HttpResults.InternalServerError(new
			{
				isError = true,
				error = new[] { ex.Message }
			});
		}
	}

	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Post, "/v1/customers")]
	public async Task<IHttpResult> Post([FromBody] CustomerDto dto, ILambdaContext context)
	{
		if (dto == null)
		{
			context.Logger.LogError($"[{nameof(Post)}]: Body is empty");

		}
		// Manual validation using data annotations
		var validationResults = new List<ValidationResult>();
		var validationContext = new ValidationContext(dto!);

		if (!Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true))
		{
			var errorMessages = validationResults.Select(vr => vr.ErrorMessage).ToList();
			context.Logger.LogError($"[{nameof(Post)}] - Validation failed: {string.Join(", ", errorMessages)}");
			return HttpResults.BadRequest(new { isError = true, errors = errorMessages });
		}

		context.Logger.LogInformation($"[{nameof(Post)}] - Body: {JsonSerializer.Serialize(dto)} ");

		try
		{
			var result = await _customerService.CreateOrUpdate(dto!);
			context.Logger.LogInformation($"[{nameof(Post)}] - Result: {JsonSerializer.Serialize(result)}");

			return HttpResults.Ok(new
			{
				isError = false,
				errors = Enumerable.Empty<string>(),
				data = result
			});
		}
		catch (Exception ex)
		{
			context.Logger.LogError($"[{nameof(Post)}] - Error");
			context.Logger.LogError($"[{nameof(Post)}] - {ex.Message}");
			return HttpResults.InternalServerError(new
			{
				isError = true,
				error = new[] { ex.Message }
			});
		}
	}


	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Put, "/v1/customers")]
	public async Task<IHttpResult> Put([FromBody] CustomerDto dto, ILambdaContext context)
	{
		if (dto == null)
		{
			context.Logger.LogError($"[{nameof(Put)}]: Body is empty");

		}
		// Manual validation using data annotations
		var validationResults = new List<ValidationResult>();
		var validationContext = new ValidationContext(dto!);

		if (!Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true))
		{
			var errorMessages = validationResults.Select(vr => vr.ErrorMessage).ToList();
			context.Logger.LogError($"[{nameof(Put)}] - Validation failed: {string.Join(", ", errorMessages)}");
			return HttpResults.BadRequest(new { isError = true, errors = errorMessages });
		}

		context.Logger.LogInformation($"[{nameof(Put)}] - Body: {JsonSerializer.Serialize(dto)} ");

		try
		{
			var result = await _customerService.CreateOrUpdate(dto!);
			context.Logger.LogInformation($"[{nameof(Put)}] - Result: {JsonSerializer.Serialize(result)}");

			return HttpResults.Ok(new
			{
				isError = false,
				errors = Enumerable.Empty<string>(),
				data = result
			});
		}
		catch (Exception ex)
		{
			context.Logger.LogError($"[{nameof(Put)}] - Error");
			context.Logger.LogError($"[{nameof(Put)}] - {ex.Message}");
			return HttpResults.InternalServerError(new
			{
				isError = true,
				error = new[] { ex.Message }
			});
		}
	}

	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Delete, "/v1/customers/{country}/{email}")]
	public async Task<IHttpResult> Delete(string country, string email, ILambdaContext context)
	{
		context.Logger.LogInformation($"[{nameof(Delete)}] - Country: {country}, Email: {email}");
		if (string.IsNullOrEmpty(country))
		{
			return HttpResults.BadRequest(new
			{
				isError = true,
				errors = new[] { "Invalid country" }
			});
		}
		if (string.IsNullOrEmpty(email))
		{
			return HttpResults.BadRequest(new
			{
				isError = true,
				errors = new[] { "Invalid email" }
			});
		}
		try
		{
			await _customerService.Delete(country, email);
			return HttpResults.Ok(new
			{
				isError = false,
				errors = Enumerable.Empty<string>()
			});
		}
		catch (Exception ex)
		{
			context.Logger.LogError($"[{nameof(Delete)}] - Error");
			context.Logger.LogError($"[{nameof(Delete)}] - {ex.Message}");
			return HttpResults.InternalServerError(new
			{
				isError = true,
				error = new[] { ex.Message }
			});
		}
	}

}