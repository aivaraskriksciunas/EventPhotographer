using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.IO.Compression;

namespace EventPhotographer.Worker.Workers;

internal class EventCompressedFileGenerator : IJob
{
    private readonly AppDbContext _dbContext;
    private readonly MediaStorageService _mediaStorageService;

    public EventCompressedFileGenerator(
        AppDbContext dbContext,
        MediaStorageService mediaStorageService)
    { 
        _dbContext = dbContext;
        _mediaStorageService = mediaStorageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var endedEvent = await _dbContext.Events
            .Where(e => e.EndDate <= DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (endedEvent == null)
        {
            return;
        }

        var media = _dbContext.MediaFiles
            .Where(m => m.Media.EventId == endedEvent.Id)
            .Where(m => m.Media.Type == MediaType.Image)
            .AsAsyncEnumerable();

        await CompressFiles(media);
    }

    private async Task CompressFiles(IAsyncEnumerable<MediaFile> files)
    {
        var tempPath = Path.GetTempFileName();
        using var zipStream = new FileStream(tempPath, FileMode.Create);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);

        try
        {
            await foreach (var file in files)
            {
                var response = await _mediaStorageService.GetFileAsync(file.Path);

                var entry = archive.CreateEntry(file.Path, CompressionLevel.Optimal);
                using var entryStream = await entry.OpenAsync();

                await response.ResponseStream.CopyToAsync(entryStream);
            }

            using var uploadStream = File.OpenRead(tempPath);
            await _mediaStorageService.UploadFile(uploadStream, "application/zip", "zippedfile.zip");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
