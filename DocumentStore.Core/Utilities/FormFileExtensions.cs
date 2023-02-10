using DocumentStore.Core.Enums;
using DocumentStore.Exceptions;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentStore.Core.Utilities
{
    public static class FormFileExtensions
    {
        private static readonly int DefaultBufferSize = 80 * 1024;
        // Set maximum file size to 5 MB.
        private static readonly int MaximumFileSize = 5242880;

        /// <summary>
        /// Asynchronously saves the contents of an uploaded file. It runs a file type check and a virus scan on the uploaded file. 
        /// </summary>
        /// <param name="formFile">The <see cref="IBrowserFile"/>.</param>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        public async static Task<FileVerificationResult> SaveAsAsync(this IBrowserFile formFile, string fileName, FileMode mode, string serverUploadPath, string tempPath, CancellationToken cancellationToken = default)
        {
            if (formFile == null)
            {
                throw new DocumentStoreException("The file chosen is empty.");
            }
            try
            {
                string filePath = Path.Combine(serverUploadPath, fileName);

                // Use extension method VerifyFile() to check whether the file conforms to the expected types and is not malicious.
                var fileVerificationResult = await formFile.VerifyFile(fileName, tempPath, cancellationToken);

                if (fileVerificationResult == FileVerificationResult.Passed)
                {
                    using var fileStream = new FileStream(filePath, mode);
                    var inputStream = formFile.OpenReadStream(MaximumFileSize, cancellationToken);

                    await inputStream.CopyToAsync(fileStream, DefaultBufferSize, cancellationToken);
                }
                return fileVerificationResult;
            }
            catch(Exception e)
            {
                throw new DocumentStoreException(e.Message, e);
            }
            finally
            {
                string tempFilePath = Path.Combine(tempPath, fileName);

                // Delete temp file, if it still exists.
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        /// <summary>
        /// Performs a check on the file type and a virus scan. Returns: 0 = file type check failed, 1 = failed the virus scan, 2 = passed.  
        /// </summary>
        /// <param name="formFile">The <see cref="IBrowserFile"/>.</param>
        /// <param name="fileName"></param>
        public async static Task<FileVerificationResult> VerifyFile(this IBrowserFile formFile, string fileName, string tempPath, CancellationToken cancellationToken = default)
        {
            string tempFilePath = Path.Combine(tempPath, fileName);

            if (formFile == null)
            {
                return FileVerificationResult.FileNotFound;
            }
            // Check if file name contains invalid characters.
            else if (formFile.Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                return FileVerificationResult.InvalidCharacters;
            }
            // Check if file name contains reserved words.
            else if (ReservedWords.WordList.Any(formFile.Name.Contains))
            {
                return FileVerificationResult.ReservedWords;
            }
            else
            {
                using var fileStream = new FileStream(tempFilePath, FileMode.Create);
                var inputStream = formFile.OpenReadStream(MaximumFileSize, cancellationToken);

                await inputStream.CopyToAsync(fileStream, DefaultBufferSize, cancellationToken);

                // Validate file type.
                if (FileVerifier.IsValidFileExtension(Path.GetFileName(tempFilePath), fileStream))
                {
                    //Run virus scan.
                    var exeLocation = @"C:\Program Files (x86)\Sophos\Sophos Anti-Virus\sav32cli.exe";
                    var scanner = new SophosScanner(exeLocation);
                    var result = scanner.Scan(tempFilePath, 10000);

                    return result == ScanResult.NoThreatFound ? FileVerificationResult.Passed : FileVerificationResult.VirusScanFailed;
                }
                else
                {
                    return FileVerificationResult.TypeCheckFailed;
                }
            }
        }
    }
}
