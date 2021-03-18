using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator.Controllers
{
    /// <summary>
    /// Health controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class HealthController : DomainBaseController
    {
        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns></returns>
        [Route("check")]
        [Produces("application/json")]
        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            return await Task.Run(() =>
            {
                return Ok(GetApiResponse(null));
            });
        }
    }
}
