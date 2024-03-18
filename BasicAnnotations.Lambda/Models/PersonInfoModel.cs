using Amazon.DynamoDBv2.DataModel;

namespace BasicAnnotations.Lambda
{
	[DynamoDBTable("PersonInfo")]
	public class PersonInfoModel
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }
		public string Email { get; set; }
		public string Country { get; set; }
		public string Phone { get; set; }
		public string JobTitle { get; set; }
	}
}
