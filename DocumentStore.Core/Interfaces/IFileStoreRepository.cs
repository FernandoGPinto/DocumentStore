using DocumentStore.Core.Dto;
using DocumentStore.Core.Enums;
using DocumentStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentStore.Core.Interfaces
{
    public interface IFileStoreRepository
    {
        Task<FileDescription?> GetSingleFileDescriptionAsync(Guid streamId, CancellationToken cancellationToken);

        Task<List<FileDescriptionShortDto>> GetFileDescriptionsAsync(Sections section, CancellationToken cancellationToken);

        Task<FileStore?> GetFileAsync(Guid streamId, CancellationToken cancellationToken);

        Task<Guid?> GetStreamIdByFileNameAsync(string fileName, CancellationToken cancellationToken);

        Task<int> DeleteFileAsync(Guid streamId, CancellationToken cancellationToken);

        Task<int> InsertFileDescriptionAsync(FileDescription fileDescription, CancellationToken cancellationToken);

        Task<int> UpdateFileDescriptionAsync(FileDescription newFileDescription, CancellationToken cancellationToken);
    }
}
