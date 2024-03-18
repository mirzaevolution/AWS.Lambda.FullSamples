using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using BasicAnnotations.Lambda.Models;
using BasicAnnotations.Lambda.Services;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BasicAnnotations.Lambda;

/// <summary>
/// A collection of sample Lambda functions that provide a REST api for doing simple math calculations. 
/// </summary>
public class Functions
{
	private readonly IPeopleInfoService _peopleInfoService;
	public Functions(IPeopleInfoService peopleInfoService)
	{
		_peopleInfoService = peopleInfoService;
	}

	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Get, "/people-info/{id}")]
	public async Task<IHttpResult> GetPersonInfo(string id, ILambdaContext context)
	{
		if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var personInfoId))
		{
			return HttpResults.BadRequest(new
			{
				error = "Invalid id"
			});
		}
		context.Logger.LogInformation($"Getting person info with id: {personInfoId}");
		var personInfo = await _peopleInfoService.GetPersonInfo(personInfoId);
		if (personInfo != null)
		{
			context.Logger.LogInformation(JsonSerializer.Serialize(personInfo));
		}
		else
		{
			context.Logger.LogError($"Person with id: {id} not found");
		}
		return HttpResults.Ok(new DataResponse<PersonInfoModel?>(personInfo));
	}

	[LambdaFunction]
	[HttpApi(LambdaHttpMethod.Post, "/people-info")]
	public async Task<IHttpResult> PostPersonInfo([FromBody] PostPersonInfoRequest request)
	{
		if (request == null)
		{
			return HttpResults.BadRequest(new
			{
				error = "Invalid payload. Payload cannot be null!"
			});
		}

		var response = await _peopleInfoService.PostPersonInfo(new PersonInfoModel
		{
			Name = request.Name,
			Email = request.Email,
			JobTitle = request.JobTitle,
			Phone = request.Phone,
			Country = request.Country,
		});
		return HttpResults.Ok(new DataResponse<PersonInfoModel?>(response));
	}

}