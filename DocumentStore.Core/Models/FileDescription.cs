using DocumentStore.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Models
{
    public class FileDescription
    {
        [Key]
        public Guid StreamId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public Sections SectionId { get; set; }
    }
}
