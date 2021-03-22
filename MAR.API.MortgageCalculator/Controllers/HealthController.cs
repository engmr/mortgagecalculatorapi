using MAR.API.MortgageCalculator.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator.Controllers
{
    /// <summary>
    /// Health controller
    /// </summary>
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class HealthController : DomainBaseController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="authTokenProvider"></param>
        /// <param name="appSettings"></param>
        public HealthController(ILoggerFactory loggerFactory,
            IAuthTokenProvider authTokenProvider,
            IOptions<AppSettings> appSettings)
            : base(loggerFactory, authTokenProvider, appSettings)
        {
        }
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
