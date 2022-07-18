namespace CleanLambdaFunction.Presentation.LambdaFunction.Mapping;

using AutoMapper;
using Application = Application.Reviews.Entities;
using Presentation = Models;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        _ = this.CreateMap<Application.Review, Presentation.Review>()
            .ForSourceMember(s => s.ReviewAuthor, o => o.DoNotValidate())
            .ForSourceMember(s => s.DateCreated, o => o.DoNotValidate())
            .ForSourceMember(s => s.DateModified, o => o.DoNotValidate());
    }
}
