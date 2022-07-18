namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

public record Author(Guid Id, string FirstName, string LastName, ICollection<Review> Reviews);
