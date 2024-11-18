namespace BasicAnnotations.Lambda.Services
{
	public interface IPeopleInfoService
	{
		Task<PersonInfoModel> GetPersonInfo(Guid id);
		Task<PersonInfoModel> PostPersonInfo(PersonInfoModel personInfo);
		Task DeletePersonInfo(Guid id);
	}
}
