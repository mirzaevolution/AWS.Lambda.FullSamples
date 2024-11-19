using System.ComponentModel.DataAnnotations;

namespace Customers.Lambda.Annotations.Models
{
	public class CustomerDto
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }

		public string Address { get; set; }
		[Required]
		public string Country { get; set; }
	}
}
