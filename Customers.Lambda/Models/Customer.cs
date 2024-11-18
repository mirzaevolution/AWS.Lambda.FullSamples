using Amazon.DynamoDBv2.DataModel;

namespace Customers.Lambda.Models
{
	[DynamoDBTable("Customers")]
	public class Customer
	{
		[DynamoDBProperty]
		public string Id { get; set; } = Guid.NewGuid().ToString();
		[DynamoDBProperty]
		public string Name { get; set; }
		[DynamoDBRangeKey]

		public string Email { get; set; }
		[DynamoDBProperty]
		public string Address { get; set; }
		[DynamoDBHashKey]
		public string Country { get; set; }
	}
}
