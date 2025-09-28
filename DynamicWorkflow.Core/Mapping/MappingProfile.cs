using AutoMapper;
using DynamicWorkflow.Core.DTOs.Department;
using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.DTOs.Workflow;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;

namespace DynamicWorkflow.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterModel, RegisterDto>().ReverseMap();
            CreateMap<LoginModel, LoginDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, ApplicationUser>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<Workflow, WorkflowDto>();
            CreateMap<WorkflowStep, WorkflowStepDto>();
        }
    }
}
