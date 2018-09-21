using Microsoft.AspNetCore.Mvc;

namespace ApiAccountService.Controllers
{
    /// <summary>
    /// summary for StatusController
    /// </summary>
    [Route("api/accounts/status")]
    public class StatusController : Controller
    {
        /// <summary>
        /// check status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            return "It's working!";
        }
    }
}
