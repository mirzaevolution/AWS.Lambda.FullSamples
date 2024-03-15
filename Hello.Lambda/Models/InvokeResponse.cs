namespace Hello.Lambda.Models
{
	public class InvokeResponse
	{
		public string OriginalMessage { get; set; }
		public string UppercasedMessage { get; set; }
		public string LowercasedMessage { get; set; }
		public string Base64EncodedMessage { get; set; }
	}
}
