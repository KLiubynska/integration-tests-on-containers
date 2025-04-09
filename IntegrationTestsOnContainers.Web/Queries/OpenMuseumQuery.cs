using MediatR;

namespace IntegrationTestsOnContainers.Web.Queries;

public record MuseumReadModel(string Name, bool IsOpened);

public record MuseumQuery(bool IsOpen) : IRequest<IReadOnlyCollection<MuseumReadModel>>;