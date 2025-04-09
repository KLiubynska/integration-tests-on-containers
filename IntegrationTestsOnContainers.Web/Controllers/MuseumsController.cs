using IntegrationTestsOnContainers.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTestsOnContainers.Web.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class MuseumsController(IMediator mediator, ILogger<MuseumsController> logger) : ControllerBase
    {
        [HttpGet("open")]
        public async Task<IEnumerable<MuseumReadModel>> GetOpen()
        {
            return await mediator.Send(new MuseumQuery(true));
        }

        [HttpGet("closed")]
        public async Task<IEnumerable<MuseumReadModel>> GetCosed()
        {
            return await mediator.Send(new MuseumQuery(false));
        }
    }
}
