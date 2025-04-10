using IntegrationTestsOnContainers.Web.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTestsOnContainers.Web.Queries;
 
public class MuseumQueryHandler(ApplicationDbContext context) : IRequestHandler<MuseumQuery, IReadOnlyCollection<MuseumReadModel>>
{
    public async Task<IReadOnlyCollection<MuseumReadModel>> Handle(MuseumQuery request, CancellationToken cancellationToken)
    {
        var museums = await context.Museums.Where(x => x.IsOpened == request.IsOpen).ToListAsync(cancellationToken);

        return museums.Select(x => new MuseumReadModel(x.Name, x.IsOpened)).ToArray();
    }
}
