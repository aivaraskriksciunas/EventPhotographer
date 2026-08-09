using EventPhotographer.Core;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Queries;
using EventPhotographer.UseCases.Events.Authorization;
using EventPhotographer.Core.Features.Content.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using EventPhotographer.UseCases.Content.Authorization;

namespace EventPhotographer.UseCases.Content.Queries;

public record GetMediaListForEventQuery : IQuery<IEnumerable<MediaModel>>
{
    public required Event Event;
    public required User User;
}

public record GetArchiveForEventQuery : IQuery<MediaModel?>
{
    public required Event Event;
    public required User User;
}

public record MediaModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public MediaParticipantModel? Participant { get; set; }
    public required IEnumerable<MediaFileModel> Files { get; set; }
}

public record MediaParticipantModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record MediaFileModel
{
    public Guid Id { get; set; }
    public required string MimeType { get; set; }
    public ulong FileSize { get; set; }
}

public class MediaQueryHandler(
    AppDbContext db,
    AuthorizationService authorizationService)
    : IQueryHandler<GetMediaListForEventQuery, IEnumerable<MediaModel>>,
    IQueryHandler<GetArchiveForEventQuery, MediaModel?>
{
    public async Task<Result<IEnumerable<MediaModel>>> QueryAsync(GetMediaListForEventQuery query, CancellationToken cancellationToken = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(query.User, query.Event, new ManageEventRequirement());
        if (!authResult.IsAuthorized)
        {
            return authResult;
        }

        var queryable = db.Media
            .Where(m => m.EventId == query.Event.Id)
            .Where(m => m.Type == MediaType.UserUpload)
            .Where(m => m.Status == MediaStatus.Validated || m.Status == null)
            .Where(m => m.Files.Any());

        return await SelectMediaModel(queryable).ToListAsync();
    }

    public async Task<Result<MediaModel?>> QueryAsync(GetArchiveForEventQuery query, CancellationToken cancellationToken = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(query.User, query.Event, new ManageEventRequirement());
        if (!authResult.IsAuthorized)
        {
            return authResult;
        }

        var queryable = db.Media
            .Where(m => m.EventId == query.Event.Id)
            .Where(m => m.Type == MediaType.Archive)
            .Where(m => m.Files.Any())
            .OrderByDescending(m => m.CreatedAt);

        return await SelectMediaModel(queryable).FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<MediaModel> SelectMediaModel(IQueryable<Media> queryable)
    {
        return queryable
            .Select(m => new MediaModel
            {
                Id = m.Id,
                CreatedAt = m.CreatedAt,
                Participant = m.Participant != null ? new MediaParticipantModel
                {
                    Id = m.Participant.Id,
                    Name = m.Participant.Name
                } : null,
                Files = m.Files.Select(f => new MediaFileModel
                {
                    Id = f.Id,
                    MimeType = f.MimeType,
                    FileSize = f.FileSize,
                }),
            });
    }
}
