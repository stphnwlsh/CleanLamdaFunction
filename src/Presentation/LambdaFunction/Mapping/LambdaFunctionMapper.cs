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
    [MapperIgnoreSource(nameof(AppAuthor.DateCreated))]
    [MapperIgnoreSource(nameof(AppAuthor.DateModified))]
    public partial PresentationAuthor ToPresentation(AppAuthor author);

    [MapperIgnoreSource(nameof(AppMovie.Reviews))]
    [MapperIgnoreSource(nameof(AppMovie.DateCreated))]
    [MapperIgnoreSource(nameof(AppMovie.DateModified))]
    private partial PresentationMovie ToPresentation(AppMovie movie);

    [MapperIgnoreSource(nameof(AppReview.ReviewAuthor))]
    [MapperIgnoreSource(nameof(AppReview.DateCreated))]
    [MapperIgnoreSource(nameof(AppReview.DateModified))]
    private partial PresentationReview ToPresentation(AppReview review);
}
