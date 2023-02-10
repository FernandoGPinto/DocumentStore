using DocumentStore.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Data
{
    public class FileStoreDBContext : DbContext
    {
        public FileStoreDBContext(DbContextOptions<FileStoreDBContext> options)
            : base(options)
        {
            
        }

        public DbSet<FileStore> FileRepository { get; set; }

        public DbSet<FileDescription> FileDescriptions { get; set; }
    }
}
