namespace CleanLambdaFunction.Presentation.LambdaFunction.Mapping;

using Riok.Mapperly.Abstractions;
using AppAuthor = Application.Authors.Entities.Author;
using AppMovie = Application.Movies.Entities.Movie;
using AppReview = Application.Reviews.Entities.Review;
using PresentationAuthor = Models.Author;
using PresentationMovie = Models.Movie;
using PresentationReview = Models.Review;

[Mapper]
internal partial class LambdaFunctionMapper
{
    public partial PresentationAuthor ToPresentation(AppAuthor author);

    [MapperIgnoreSource(nameof(AppMovie.Reviews))]
    private partial PresentationMovie ToPresentation(AppMovie movie);

    [MapperIgnoreSource(nameof(AppReview.ReviewAuthor))]
    private partial PresentationReview ToPresentation(AppReview review);
}
