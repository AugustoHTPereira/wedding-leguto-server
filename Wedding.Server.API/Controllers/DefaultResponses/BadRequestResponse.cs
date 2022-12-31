using Microsoft.AspNetCore.Mvc;

namespace Wedding.Server.API.Controllers.DefaultResponses;

public class BadRequestResponse
{
    public BadRequestResponse()
    {
        
    }

    public BadRequestResponse(string message)
    {
        Message = message;
    }

    public string Message { get; set; } = "Bad request";

    public static BadRequestObjectResult CreateResponse() => new BadRequestObjectResult(new BadRequestResponse());
    public static BadRequestObjectResult CreateResponse(string message) => new BadRequestObjectResult(new BadRequestResponse(message));
}