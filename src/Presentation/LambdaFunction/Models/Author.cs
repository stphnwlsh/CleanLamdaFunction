namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

public record Author(Guid Id, string FirstName, string LastName, int ReviewCount = 0, double? AverageStars = null);
