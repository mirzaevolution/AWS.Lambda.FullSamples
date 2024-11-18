using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace BasicAnnotations.Lambda.Services
{
	public class PeopleInfoService : IPeopleInfoService
	{
		private readonly IDynamoDBContext _dynamoDbContext;
		public PeopleInfoService(IAmazonDynamoDB amazonDynamoDB)
		{
			_dynamoDbContext = new DynamoDBContext(amazonDynamoDB);
		}

		public async Task<PersonInfoModel> GetPersonInfo(Guid id)
		{
			var response = await _dynamoDbContext.LoadAsync<PersonInfoModel>(id);
			return response;
		}

		public async Task<PersonInfoModel> PostPersonInfo(PersonInfoModel personInfo)
		{
			await _dynamoDbContext.SaveAsync<PersonInfoModel>(personInfo);
			return personInfo;
		}

		public async Task DeletePersonInfo(Guid id)
		{
			await _dynamoDbContext.DeleteAsync<PersonInfoModel>(id);
		}
	}
}
