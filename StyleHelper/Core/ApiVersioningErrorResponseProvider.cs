using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using RHerber.Common.AspNetCore.Core;

namespace StyleHelper.Core;

// TODO: Move to RHerber.Common.AspNetCore common package
public class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
{
    public override IActionResult CreateResponse(ErrorResponseContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var problemDetails = new ProblemDetailsWithErrors(context.Message ?? context.MessageDetail ?? "Unsupported API version.", StatusCodes.Status400BadRequest, context.Request);

        return new BadRequestObjectResult(problemDetails);
    }
}
