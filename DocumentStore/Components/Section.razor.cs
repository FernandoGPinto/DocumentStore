using DocumentStore.Core.Enums;
using DocumentStore.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using DocumentStore.Core.Interfaces;
using Serilog;
using DocumentStore.Core.Dto;

namespace DocumentStore.Components
{
    public partial class Section
    {
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public IFileStoreService FileStoreService { get; set; }
        [Parameter] public Sections SectionName { get; set; }

        private List<FileDescriptionShortDto>? _fileDescriptionsShort;
        private bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            _fileDescriptionsShort = await FileStoreService.GetFileDescriptionsAsync(SectionName, CancellationToken);
        }

        private async Task DownloadFile(Guid streamId, string fileName)
        {
            _isLoading = true;

            try
            {
                // Get file from filetable.
                var file = await FileStoreService.GetFileAsync(streamId, CancellationToken);

                _isLoading = false;

                // Save file on client.
                await FileUtil.SaveAs(JS, fileName, file.FileStream);
            }
            catch (Exception e)
            {
                _isLoading = false;

                await JS.InvokeAsync<object>("swal", "Unable to Download File", "Please try again or contact I.T. support.", "error");
                Log.Error(e, e.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        private async Task DeleteFile(Guid streamId, string fileName)
        {
            // Confirm whether user wants to delete file.
            var confirmed = await JS.InvokeAsync<object>("swalNotification", "Are you sure?", $"You are about to delete \"{ fileName }\".", "warning", true, true);

            if (confirmed is not null && confirmed.ToString() == "True")
            {
                string userNotificationTitle = "Unable to Delete File";
                string userNotificationMessage = "Please try again or contact I.T. Support.";
                string userNotificationIcon = "error";

                _isLoading = true;

                try
                {
                    // Delete file from filetable.
                    var result = await FileStoreService.DeleteFileAsync(streamId, CancellationToken);
                    if (result > 0)
                    {
                        // Remove from list displayed to user.
                        var item = _fileDescriptionsShort.Where(x => x.StreamId == streamId).SingleOrDefault();
                        _fileDescriptionsShort.Remove(item);

                        StateHasChanged();

                        userNotificationTitle = "File Deleted";
                        userNotificationMessage = $"\"{ fileName}\" has been deleted.";
                        userNotificationIcon = "info";
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                }
                finally
                {
                    _isLoading = false;

                    // Notify user of result.
                    await JS.InvokeAsync<object>("swal", userNotificationTitle, userNotificationMessage, userNotificationIcon);
                }
            }
        }
    }
}
