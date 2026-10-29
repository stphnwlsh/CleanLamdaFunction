namespace CleanLambdaFunction.Presentation.LambdaFunction.Models;

public record Movie(Guid Id, string Title, int ReviewCount = 0, double? AverageStars = null);
