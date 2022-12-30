using Microsoft.AspNetCore.Mvc;

namespace Wedding.Server.API.Controllers.DefaultResponses;

public class BadRequestResponse
{
    public string Message { get; set; } = "Bad request";

    public static BadRequestObjectResult CreateResponse() => new BadRequestObjectResult(new BadRequestResponse());
}