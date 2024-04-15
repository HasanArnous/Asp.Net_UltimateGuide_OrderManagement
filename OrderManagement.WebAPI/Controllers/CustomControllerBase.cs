using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class CustomControllerBase : ControllerBase
{
}
