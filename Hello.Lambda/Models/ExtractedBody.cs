using System.Text.Json.Serialization;

namespace Hello.Lambda.Models
{
	public class ExtractedBody
	{
		[JsonPropertyName("body")]
		public object Body { get; set; }
	}
}
