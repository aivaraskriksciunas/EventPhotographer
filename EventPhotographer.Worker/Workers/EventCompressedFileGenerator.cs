using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.IO.Compression;
using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.Worker.Workers;

[DisallowConcurrentExecution]
internal class EventCompressedFileGenerator : IJob
{
    private readonly AppDbContext _dbContext;
    private readonly MediaStorageService _mediaStorageService;
    private readonly MediaService _mediaService;

    public EventCompressedFileGenerator(
        AppDbContext dbContext,
        MediaStorageService mediaStorageService,
        MediaService mediaService)
    { 
        _dbContext = dbContext;
        _mediaStorageService = mediaStorageService;
        _mediaService = mediaService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var endedEvent = await FindEndedEventWithoutArchive();

        if (endedEvent == null)
        {
            return;
        }

        var media = _dbContext.MediaFiles
            .Where(f => f.Media.Type == MediaType.UserUpload)
            .Where(f => f.Media.EventId == endedEvent.Id)
            .ToAsyncEnumerable();

        var path = await CreateZipFile(media);
        if (path == null)
        {
            return;
        }

        try
        {
            await UploadCompressedFile(endedEvent, path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private async Task<Event?> FindEndedEventWithoutArchive()
    {
        var cutoffDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(14));
        return await _dbContext.Events
            .Where(e => e.EndDate <= DateTime.UtcNow)
            .Where(e => e.EndDate >= cutoffDate) // Only consider events that ended recently
            .Where(e => !e.Media.Any(m => m.Type == MediaType.Archive))
            .Where(e => e.Media.Any(m => m.Type == MediaType.UserUpload))
            .FirstOrDefaultAsync();
    }

    private async Task<string?> CreateZipFile(IAsyncEnumerable<MediaFile> files)
    {
        if (!await files.AnyAsync())
        {
            return null;
        }

        var tempPath = Path.GetTempFileName();
        try
        {
            using var zipStream = new FileStream(tempPath, FileMode.Create);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);

            await foreach (var file in files)
            {
                var response = await _mediaStorageService.GetFileAsync(file.Path);

                var entry = archive.CreateEntry(file.Path, CompressionLevel.Optimal);
                using var entryStream = await entry.OpenAsync();

                await response.ResponseStream.CopyToAsync(entryStream);
            }
        }
        catch (Exception)
        {
            File.Delete(tempPath);
            return null;
        }

        return tempPath;
    }

    private async Task UploadCompressedFile(Event @event, string zipFile)
    {
        using var uploadStream = File.OpenRead(zipFile);

        var media = await _mediaService.CreateArchive(@event);
        await _mediaService.UploadFile(media, uploadStream, ".zip");
    }
}
