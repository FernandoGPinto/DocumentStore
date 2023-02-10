using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Utilities
{
    public class FileVerifier
    {
        private static readonly Dictionary<string, List<byte[]>> FileSignature = new()
                    {
                    { ".DOC", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
                    { ".DOCX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
                    { ".PDF", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
                    { ".PPT", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
                    { ".PPTX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
                    { ".PNG", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
                    { ".JPG", new List<byte[]>
                                    {
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                                    }
                                    },
                    { ".JPEG", new List<byte[]>
                                        {
                                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                                        }
                                        },
                    { ".XLS", new List<byte[]>
                                            {
                                              new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                                              new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 },
                                              new byte[] { 0xFD, 0xFF, 0xFF, 0xFF }
                                            }
                                            },
                    { ".XLSX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } }
                };

        /// <summary>
        /// Validates the file format signature for the file provided, to prevent the upload of a masqueraded file. If the file extension is not contained in the approved list or the file header bytes do not match the expected signature returns false. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static bool IsValidFileExtension(string fileName, Stream fileStream)
        {
            if (string.IsNullOrEmpty(fileName) || fileStream == null || fileStream.Length == 0)
            {
                return false;
            }

            bool flag = false;
            string ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }

            ext = ext.ToUpperInvariant();

            // Select relevant signature based on the file extension.
            // If not present return false.
            if (!FileSignature.TryGetValue(ext, out List<byte[]> signature))
            {
                return false;
            }

            // Take length of the longest byte[] within 'signature'.
            int signatureLength = signature.Max(m => m.Length);

            // Read file stream up to the signatureLength.
            fileStream.Position = 0;
            var reader = new BinaryReader(fileStream);
            var headerBytes = reader.ReadBytes(signatureLength);

            // Verify if the file signature matches the expected signature.
            flag = signature.Any(s => headerBytes.Take(s.Length)
                        .SequenceEqual(s));

            return flag;
        }
    }
}
