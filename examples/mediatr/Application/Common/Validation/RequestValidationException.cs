using System.Linq;
namespace MediatrCodeSideExamples.Application.Common.Validation;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(IEnumerable<string> errors)
        : base("Validation failed.")
    {
        Errors = errors.ToArray();
    }

    public IReadOnlyCollection<string> Errors { get; }
}

