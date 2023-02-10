using AutoMapper;
using DocumentStore.Core.Dto;
using DocumentStore.Core.Enums;
using DocumentStore.Core.Interfaces;
using DocumentStore.Core.Models;
using DocumentStore.Core.Utilities;
using DocumentStore.Exceptions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DocumentStore.Services
{
    public class FileStoreService : IFileStoreService
    {
        private readonly IFileStoreRepository _fileStoreRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public FileStoreService(IFileStoreRepository fileStoreRepository, IConfiguration config, IMapper mapper) 
        {
            _fileStoreRepository = fileStoreRepository;
            _config = config;
            _mapper = mapper;
        }

        public async Task<FileDescriptionDto> GetSingleFileDescriptionAsync(Guid streamId, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileDescription = await _fileStoreRepository.GetSingleFileDescriptionAsync(streamId, cancellationToken);
                return _mapper.Map<FileDescriptionDto>(fileDescription);
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
                return await _fileStoreRepository.GetFileDescriptionsAsync(section, cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error retrieving file descriptions", e);
            }
        }

        public async Task<FileStoreDto> GetFileAsync(Guid streamId, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileStore = await _fileStoreRepository.GetFileAsync(streamId, cancellationToken);
                return _mapper.Map<FileStoreDto>(fileStore);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error retrieving file", e);
            }
        }

        public async Task<int> DeleteFileAsync(Guid streamId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _fileStoreRepository.DeleteFileAsync(streamId, cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error deleting file", e);
            }
        }

        public async Task<FileVerificationResult> SaveFileAsync(IBrowserFile file, FileDescriptionDto fileDescription, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileVerificationResult = await file.SaveAsAsync(file.Name, FileMode.CreateNew, _config["ApplicationConfiguration:ServerUploadFolder"], _config["ApplicationConfiguration:TemporaryFilePath"], cancellationToken);

                if (fileVerificationResult == FileVerificationResult.Passed)
                {
                    // Get the details from the record inserted when the file was copied to the filetable directory.
                    var streamId = await _fileStoreRepository.GetStreamIdByFileNameAsync(file.Name, cancellationToken);

                    if (streamId.HasValue)
                    {
                        // Use the above details to set the file description stream id.
                        fileDescription.StreamId = streamId.Value;

                        // Save file description.
                        await _fileStoreRepository.InsertFileDescriptionAsync(_mapper.Map<FileDescription>(fileDescription), cancellationToken);
                    }
                }

                return fileVerificationResult;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error saving file", e);
            }
        }

        public async Task<FileVerificationResult> UpdateFileAsync(IBrowserFile file, FileDescriptionDto fileDescription, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileVerificationResult = await file.SaveAsAsync(file.Name, FileMode.Create, _config["ApplicationConfiguration:ServerUploadFolder"], _config["ApplicationConfiguration:TemporaryFilePath"], cancellationToken);

                if (fileVerificationResult == FileVerificationResult.Passed)
                {
                    await _fileStoreRepository.UpdateFileDescriptionAsync(_mapper.Map<FileDescription>(fileDescription), cancellationToken);
                }

                return fileVerificationResult;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new DocumentStoreException("Error updating file", e);
            }
        }
    }
}
