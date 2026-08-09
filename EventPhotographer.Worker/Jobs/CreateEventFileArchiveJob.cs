using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Content.Commands;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace EventPhotographer.Worker.Workers;

internal class CreateEventFileArchiveJob
{
    private readonly AppDbContext _dbContext;
    private readonly MediaStorageService _mediaStorageService;
    private readonly ICommandHandler<UploadFileCommand, MediaFile> _uploadHandler;

    public CreateEventFileArchiveJob(
        AppDbContext dbContext,
        MediaStorageService mediaStorageService,
        ICommandHandler<UploadFileCommand, MediaFile> uploadHandler)
    {
        _dbContext = dbContext;
        _mediaStorageService = mediaStorageService;
        _uploadHandler = uploadHandler;
    }

    public async Task Execute(Guid eventId)
    {
        var endedEvent = await _dbContext.Events
            .Where(e => e.Id == eventId)
            .FirstOrDefaultAsync();

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

        var uploadResult = await _uploadHandler.HandleAsync(new UploadFileCommand
        {
            Event = @event,
            MediaType = MediaType.Archive,
            Stream = uploadStream,
            FileContentTypeInfo = FileContentTypeReader.GetFileTypeFromExtension(".zip")!,
        });

        if (!uploadResult.IsSuccess)
        {
            throw new Exception($"Failed to upload compressed file for event {@event.Id}: {uploadResult.Error.Code}");
        }
    }
}
