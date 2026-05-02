using Api.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/rbac")]
    [ApiController]
    public class RBACTestController : ApiControllerBase
    {
        [HttpGet("norestriction")]
        public IActionResult NoRestrictionEndpoint()
        {
            return Ok("This endpoint is open to everyone.");
        }

        [HttpGet("admin")]
        [HasPermission("AdminAccess")]
        public IActionResult AdminEndpoint()
        {
            return Ok("Welcome, Admin!");
        }
    }
}
