using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Data")]
    public class DataController : ApiController
    {
        [Route("Employees")]
        [HttpGet]
        public async Task<IHttpActionResult> Employees()
        {
            return Ok(new[] { "Data for employee John Doe", "Data for employee Jane Doe" });
        }
    }
}