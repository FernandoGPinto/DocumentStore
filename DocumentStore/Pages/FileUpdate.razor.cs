using DocumentStore.Core.Enums;
using DocumentStore.Core.Utilities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog;
using System;
using DocumentStore.Core.Interfaces;
using DocumentStore.Core.Dto;
using DocumentStore.Exceptions;

namespace DocumentStore.Pages
{
    public partial class FileUpdate
    {
        [Inject] IConfiguration Config { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public IFileStoreService FileStoreService { get; set; }
        [Parameter] public string StreamId { get; set; }

        private FileDescriptionDto _newFileDescription = new();
        private FileDescriptionDto _oldFileDescription = new();
        private IBrowserFile _uploadFile;
        private string _fileUploadLabel = "Choose new file (max 5 MB)";
        private bool _isLoading = false;
        private string _loadingText = "";

        protected override async Task OnInitializedAsync()
        {
            // Get the old file description.
            _oldFileDescription = await FileStoreService.GetSingleFileDescriptionAsync(Guid.Parse(StreamId), CancellationToken);

            // Set the description of the updated file. This will be shown to the user and enable the user to keep the description unchanged.
            _newFileDescription.Description = _oldFileDescription.Description;
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            _uploadFile = e.File;
            _newFileDescription.FileName = e.File.Name;
            _fileUploadLabel = e.File.Name;
        }

        public async Task UpdateFile()
        {
            string fileName = _uploadFile.Name;
            string userNotificationTitle = "Unable to Upload";
            string userNotificationMessage = "Please try again or contact I.T. Support.";
            string userNotificationIcon = "error";

            _isLoading = true;

            // Populate new file description.
            _newFileDescription.StreamId = _oldFileDescription.StreamId;
            _newFileDescription.SectionId = _oldFileDescription.SectionId;

            try
            {
                // Update only if files have the same name and extension.
                if (_oldFileDescription.FileName == _uploadFile.Name)
                {
                    _loadingText = "Validating and updating file";

                    // Update file.
                    var fileVerificationResult = await FileStoreService.UpdateFileAsync(_uploadFile, _newFileDescription, CancellationToken);

                    // Check passed and file saved.
                    if (fileVerificationResult == FileVerificationResult.Passed)
                    {
                        userNotificationTitle = "Update Sucessful!";
                        userNotificationMessage = $"\"{ fileName }\" was updated successfully.";
                        userNotificationIcon = "success";
                    }
                    // Check failed.
                    else if (fileVerificationResult == FileVerificationResult.FileNotFound)
                    {
                        userNotificationMessage = "Please choose a new file to be replace the old one.";
                    }
                    else if (fileVerificationResult == FileVerificationResult.TypeCheckFailed)
                    {
                        userNotificationMessage = "The file type is not an accepted type:\n .doc .docx .pdf .ppt .pptx .png .jpg .jpeg .xls .xlsx\n\nIf your file extension is on this list please contact I.T. Support.";
                    }
                    else if (fileVerificationResult == FileVerificationResult.VirusScanFailed)
                    {
                        userNotificationMessage = "The selected file has failed the virus scan.\nPlease upload a different file.";
                    }
                    else if (fileVerificationResult == FileVerificationResult.InvalidCharacters)
                    {
                        userNotificationMessage = $"Your file name contains one or more of the following invalid characters  { string.Join(" ", Path.GetInvalidFileNameChars()) }";
                    }
                    else if (fileVerificationResult == FileVerificationResult.ReservedWords)
                    {
                        userNotificationMessage = $"Your file name contains one or more of the following reserved words: { string.Join(", ", ReservedWords.WordList) }";
                    }
                    else
                    {
                        userNotificationMessage = "The file type check and virus scan could not be completed.\nPlease try again or contact I.T. Support.";
                    }
                }
                else
                {
                    userNotificationMessage = "The file names and extensions do not match.\nIf you need to upload a different file to replace this one, please delete and use the 'File Upload' function.";
                }
            }
            catch (DocumentStoreException e)
            {
                userNotificationMessage = e.Message;
                Log.Error(e, e.Message);
            }
            finally
            {
                // Notify user of result.
                await JS.InvokeAsync<object>("swal", userNotificationTitle, userNotificationMessage, userNotificationIcon);

                _isLoading = false;
            }
        }
    }
}
