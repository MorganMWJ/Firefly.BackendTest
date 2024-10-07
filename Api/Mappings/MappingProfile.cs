using Api.DTOs;
using AutoMapper;
using Domain.Model;
using System.Collections.ObjectModel;

namespace Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Define your object mappings here
        CreateMap<ClassDto, Class>()
            .ForMember(dest => dest.Students, opt => opt.MapFrom(src => new Collection<Student>()));
        CreateMap<Class, ClassDto>();

        CreateMap<StudentDto, Student>()
            .ForMember(dest => dest.Classes, opt => opt.MapFrom(src => new Collection<Class>()));
        CreateMap<Student, StudentDto>();

        CreateMap<TeacherDto, Teacher>()
            .ForMember(dest => dest.Classes, opt => opt.MapFrom(src => new Collection<Class>()));
        CreateMap<Teacher, TeacherDto>();
    }
}
