using Microsoft.AspNetCore.Mvc;

namespace Wedding.Server.API.Controllers.DefaultResponses;

public class NotFoundResponse
{
    public string Message { get; set; } = "Not found";

    public static NotFoundObjectResult CreateResponse() => new NotFoundObjectResult(new NotFoundResponse());
}
