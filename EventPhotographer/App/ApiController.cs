using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
}
