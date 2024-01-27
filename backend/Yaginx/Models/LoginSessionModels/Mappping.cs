using AutoMapper;

namespace Yaginx.Models.LoginSessionModels
{

	public class Mappping : Profile
	{
		public Mappping()
		{
			//CreateMap<SysUserEntity, LoginResult>();
			CreateMap<LoginResultDto, LoginResult>()
				.ForMember(d => d.Name, mo => mo.MapFrom(s => s.DisplayName));
		}
	}
}
