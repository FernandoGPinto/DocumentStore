using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentStore.Core.Dto
{
    public class FileStoreDto
    {
        public Guid StreamId { get; set; }
        public byte[] FileStream { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset LastWriteTime { get; set; }
        public DateTimeOffset LastAccessTime { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsOffline { get; set; }
        public bool IsHidden { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsArchive { get; set; }
        public bool IsSystem { get; set; }
        public bool IsTemporary { get; set; }
    }
}
