using DocumentStore.Core.Dto;
using DocumentStore.Core.Enums;
using DocumentStore.Core.Interfaces;
using DocumentStore.Core.Models;
using DocumentStore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DocumentStore.Data.Repositories
{
    public class FileStoreRepository : IFileStoreRepository
    {
        private IDbContextFactory<FileStoreDBContext> _contextFactory;

        public FileStoreRepository(IDbContextFactory<FileStoreDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<FileDescription?> GetSingleFileDescriptionAsync(Guid streamId, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                return await context.FileDescriptions
                    .Where(x => x.StreamId == streamId)
                    .SingleOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error retrieving single file description", e);
            }
        }

        public async Task<List<FileDescriptionShortDto>> GetFileDescriptionsAsync(Sections section, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                return await context.FileDescriptions
                .Where(x => x.SectionId == section)
                .Select(s => new FileDescriptionShortDto
                {
                    StreamId = s.StreamId,
                    FileName = s.FileName,
                    Description = s.Description
                }).ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error retrieving file descriptions", e);
            }
        }

        public async Task<FileStore?> GetFileAsync(Guid streamId, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                return await context.FileRepository.SingleOrDefaultAsync(s => s.StreamId == streamId, cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error retrieving file", e);
            }
        }

        public async Task<Guid?> GetStreamIdByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                return (await context.FileRepository.SingleOrDefaultAsync(s => s.Name == fileName, cancellationToken))?.StreamId;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error retrieving stream id by file name", e);
            }
        }

        public async Task<int> DeleteFileAsync(Guid streamId, CancellationToken cancellationToken = default)
        {
            try
            {
                FileStore file = new()
                {
                    StreamId = streamId
                };

                using var context = await _contextFactory.CreateDbContextAsync();
                context.FileRepository.Remove(file);

                return await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error deleting file", e);
            }
        }

        public async Task<int> InsertFileDescriptionAsync(FileDescription fileDescription, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                context.FileDescriptions.Add(fileDescription);

                return await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error inserting file", e);
            }
        }

        public async Task<int> UpdateFileDescriptionAsync(FileDescription newFileDescription, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                context.FileDescriptions.Update(newFileDescription);

                return await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error updating file", e);
            }
        }
    }
}
