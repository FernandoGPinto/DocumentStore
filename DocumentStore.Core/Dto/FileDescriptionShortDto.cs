using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentStore.Core.Dto
{
    public class FileDescriptionShortDto
    {
        public Guid StreamId { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
    }
}
