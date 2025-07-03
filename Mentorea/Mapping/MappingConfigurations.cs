


using Mentorea.Contracts.Fields;
using Mentorea.Contracts.Session;
using Mentorea.Contracts.Users;
using Mentorea.Entities;

namespace Mentorea.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MenteeRegisterRequest, ApplicationUser>()
                .Map(des => des.UserName, src => src.Email)
                .Map(des => des.NormalizedUserName, src => src.Email.ToUpper())
                .Map(des => des.NormalizedEmail, src => src.Email.ToUpper())
                .Map(des => des.PirthDate, src => new DateOnly(src.PirthDate.Year, src.PirthDate.Month, src.PirthDate.Day)); 

            config.NewConfig<MentorRegisterRequest, ApplicationUser>()
                .Map(des => des.UserName, src => src.Email)
                .Map(des => des.NormalizedUserName, src => src.Email.ToUpper())
                .Map(des => des.NormalizedEmail, src => src.Email.ToUpper())
                .Map(des=>des.PirthDate,src=>new DateOnly(src.PirthDate.Year,src.PirthDate.Month,src.PirthDate.Day)); 

            config.NewConfig<CreateUserRequest, ApplicationUser>()
                .Map(des => des.UserName, src => src.Email)
                .Map(des => des.NormalizedUserName, src => src.Email.ToUpper())
                .Map(des => des.NormalizedEmail, src => src.Email.ToUpper())
                .Map(des => des.PirthDate, src => new DateOnly(src.PirthDate.Year, src.PirthDate.Month, src.PirthDate.Day));

            config.NewConfig<Field, FieldSheep>()
                .Map(des => des.Name, src => src.FieldName);

            config.NewConfig<Field, SingleFieldResponse>()
                .Map(des => des.Name, src => src.FieldName);

            config.NewConfig<Session, SessionResponse>()
                .Map(des => des.Status, src => src.Status.ToString());
        }
    }
}
