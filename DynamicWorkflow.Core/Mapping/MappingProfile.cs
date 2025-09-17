using AutoMapper;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DynamicWorkflow.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterModel, RegisterDto>().ReverseMap();
            CreateMap<LoginModel, LoginDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}
