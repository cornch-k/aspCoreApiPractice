using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace aspCoreApiPractice.Controllers
{
    [Route("/APIExample")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("Hello")]
        [HttpGet]
        public string Index()
        {
            return "HELLO, WORLD!";
        }
    }
}