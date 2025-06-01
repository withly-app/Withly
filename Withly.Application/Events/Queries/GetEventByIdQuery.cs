using MediatR;
using Withly.Application.Events.Dtos;

namespace Withly.Application.Events.Queries;

public record GetEventByIdQuery(Guid EventId) : IRequest<EventDetailsDto?>;
