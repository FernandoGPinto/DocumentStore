using DocumentStore.Core.Enums;
using DocumentStore.Core.Utilities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Serilog;
using System;
using DocumentStore.Core.Interfaces;
using Microsoft.AspNetCore.Components;
using DocumentStore.Core.Dto;
using DocumentStore.Exceptions;

namespace DocumentStore.Pages
{
    public partial class FileUpload
    {
        [Inject] IConfiguration Config { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public IFileStoreService FileStoreService { get; set; }

        private FileDescriptionDto _fileDescription = new();
        private IBrowserFile _uploadFile;
        private string _fileUploadLabel = "Choose file (max 5 MB)";
        private string _sectionName;
        private bool _isLoading = false;
        private string _loadingText = "";

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            _uploadFile = e.File;
            _fileDescription.FileName = e.File.Name;
            _fileUploadLabel = e.File.Name;
        }

        public async Task UploadFile()
        {
            string fileName = _uploadFile.Name;
            string userNotificationTitle = "Unable to Upload";
            string userNotificationMessage = "Please try again or contact I.T. Support.";
            string userNotificationIcon = "error";

            _isLoading = true;

            _fileDescription.SectionId = (Sections)Enum.Parse(typeof(Sections), _sectionName);

            try
            {
                _loadingText = "Validating and uploading file";

                // Upload to filetable using extension method SaveAsAsync() in FormFileExtensions.cs.
                var fileVerificationResult = await FileStoreService.SaveFileAsync(_uploadFile, _fileDescription, CancellationToken);

                // Check passed and file saved.
                if (fileVerificationResult == FileVerificationResult.Passed)
                {
                    userNotificationTitle = "Upload Sucessful!";
                    userNotificationMessage = $"\"{fileName}\" was uploaded successfully.";
                    userNotificationIcon = "success";
                }
                // Check failed.
                else if (fileVerificationResult == FileVerificationResult.FileNotFound)
                {
                    userNotificationMessage = "Please choose a file to be uploaded.";
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
                    userNotificationMessage = $"Your file name contains one or more of the following invalid characters {string.Join(" ", Path.GetInvalidFileNameChars())} ";
                }
                else if (fileVerificationResult == FileVerificationResult.ReservedWords)
                {
                    userNotificationMessage = $"Your file name contains one or more of the following invalid characters {string.Join(", ", ReservedWords.WordList)}";
                }
                else
                {
                    userNotificationMessage = "The file type check and virus scan could not be completed.\nPlease try again or contact I.T. Support.";
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
