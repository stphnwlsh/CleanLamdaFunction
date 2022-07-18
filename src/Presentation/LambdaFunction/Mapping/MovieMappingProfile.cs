namespace CleanLambdaFunction.Presentation.LambdaFunction.Mapping;

using AutoMapper;
using Application = Application.Movies.Entities;
using Presentation = Models;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        _ = this.CreateMap<Application.Movie, Presentation.Movie>()
            .ForSourceMember(s => s.Reviews, o => o.DoNotValidate())
            .ForSourceMember(s => s.DateCreated, o => o.DoNotValidate())
            .ForSourceMember(s => s.DateModified, o => o.DoNotValidate());
    }
}
