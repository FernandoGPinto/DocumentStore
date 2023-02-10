using DocumentStore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentStore.Core.Dto
{
    public class FileDescriptionDto
    {
        public Guid StreamId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public Sections SectionId { get; set; }
    }
}
