using DocumentStore.Core.Dto;
using DocumentStore.Core.Enums;
using DocumentStore.Core.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentStore.Core.Interfaces
{
    public interface IFileStoreService
    {
        Task<FileDescriptionDto> GetSingleFileDescriptionAsync(Guid streamId, CancellationToken cancellationToken);

        Task<List<FileDescriptionShortDto>> GetFileDescriptionsAsync(Sections section, CancellationToken cancellationToken);

        Task<FileStoreDto> GetFileAsync(Guid streamId, CancellationToken cancellationToken);

        Task<int> DeleteFileAsync(Guid streamId, CancellationToken cancellationToken);

        Task<FileVerificationResult> SaveFileAsync(IBrowserFile file, FileDescriptionDto fileDescription, CancellationToken cancellationToken);

        Task<FileVerificationResult> UpdateFileAsync(IBrowserFile file, FileDescriptionDto fileDescription, CancellationToken cancellationToken);
    }
}
