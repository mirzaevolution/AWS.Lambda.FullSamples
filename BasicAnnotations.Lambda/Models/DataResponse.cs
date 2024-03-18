namespace BasicAnnotations.Lambda.Models
{
	public class DataResponse<T>
	{
		public DataResponse(T value)
		{
			Value = value;
		}
		public T Value { get; set; }
	}
}
