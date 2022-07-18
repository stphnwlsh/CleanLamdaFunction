namespace CleanLambdaFunction.Presentation.LambdaFunction.Mapping;

using AutoMapper;
using Application = Application.Authors.Entities;
using Presentation = Models;

public class AuthorMappingProfile : Profile
{
    public AuthorMappingProfile()
    {
        _ = this.CreateMap<Application.Author, Presentation.Author>()
            .ForSourceMember(s => s.DateCreated, o => o.DoNotValidate())
            .ForSourceMember(s => s.DateModified, o => o.DoNotValidate());
    }
}
