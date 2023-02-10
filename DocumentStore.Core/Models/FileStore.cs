using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Models
{
    public class FileStore
    {
        [Key]
        [Column("stream_id")]
        public Guid StreamId { get; set; }
        [Column("file_stream")]
        public byte[] FileStream { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("file_type")]
        public string FileExtension { get; set; }
        [Column("cached_file_size")]
        public long? CachedFileSize { get; set; }
        [Column("creation_time")]
        public DateTimeOffset CreationTime { get; set; }
        [Column("last_write_time")]
        public DateTimeOffset LastWriteTime { get; set; }
        [Column("last_access_time")]
        public DateTimeOffset LastAccessTime { get; set; }
        [Column("is_directory")]
        public bool IsDirectory { get; set; }
        [Column("is_offline")]
        public bool IsOffline { get; set; }
        [Column("is_hidden")]
        public bool IsHidden { get; set; }
        [Column("is_readonly")]
        public bool IsReadonly { get; set; }
        [Column("is_archive")]
        public bool IsArchive { get; set; }
        [Column("is_system")]
        public bool IsSystem { get; set; }
        [Column("is_temporary")]
        public bool IsTemporary { get; set; }
    }
}
