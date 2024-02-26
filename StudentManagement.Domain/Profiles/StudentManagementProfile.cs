using AutoMapper;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
using ContractsCourse = StudentManagement.Contracts.Models.Course;

namespace StudentManagement.Domain.Profiles
{
    internal class StudentManagementProfile : Profile
    {
        public StudentManagementProfile()
        {
            _ = CreateMap<DatabaseCourse, ContractsCourse>()
                .ForMember(dest => dest.AssigneeEmail, src =>
                {
                    src.PreCondition(t => t.Assignee != null);
                    src.MapFrom(t => t.Assignee.Email);
                });
        }
    }
}