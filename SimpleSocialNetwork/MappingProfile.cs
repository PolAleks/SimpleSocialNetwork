using AutoMapper;
using SimpleSocialNetwork.BLL.ViewModels.Account;
using SimpleSocialNetwork.DAL.Entity;
using System;

namespace SimpleSocialNetwork
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(u => u.BirthDate, opt => opt.MapFrom( c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
                .ForMember(u => u.Email, opt => opt.MapFrom(c => c.EmailReg))
                .ForMember(u => u.UserName, opt => opt.MapFrom(c => c.Login));
            CreateMap<LoginViewModel, User>();
            CreateMap<UserEditViewModel, User>();
            CreateMap<User, UserEditViewModel>()
                .ForMember(uedm => uedm.UserId, opt => opt.MapFrom(c => c.Id));
        }
    }
}
