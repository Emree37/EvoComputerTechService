using AutoMapper;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.MapperProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<ApplicationUser, UserProfileViewModel>().ReverseMap();
            //CreateMap<ApplicationUser, UserProfileViewModel>();
        }
    }
}
