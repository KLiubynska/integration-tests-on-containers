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
        public Task<IReadOnlyCollection<MuseumReadModel>> GetOpen() =>
            mediator.Send(new MuseumQuery(true));

        [HttpGet("closed")]
        public Task<IReadOnlyCollection<MuseumReadModel>> GetCosed() =>
            mediator.Send(new MuseumQuery(false));
    }
}
