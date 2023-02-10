using DocumentStore.Core.Models;
using DocumentStore.Data.Repositories;
using DocumentStore.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DocumentStore.Data.Tests
{
    public class TestDbContextFactory : IDbContextFactory<FileStoreDBContext>
    {
        private readonly DbContextOptions<FileStoreDBContext> _options;

        public TestDbContextFactory(string databaseName = "InMemoryTest")
        {
            _options = new DbContextOptionsBuilder<FileStoreDBContext>()
                .UseInMemoryDatabase(databaseName: databaseName).UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .Options;
        }

        public FileStoreDBContext CreateDbContext()
        {
            return new FileStoreDBContext(_options);
        }
    }

    public abstract class FileStoreRepositoryTests
    {
        private readonly IDbContextFactory<FileStoreDBContext> _contextFactory;
        protected readonly FileStoreRepository _fileStoreRepository;

        protected FileStoreRepository FileStoreRepository { get { return _fileStoreRepository; } }

        public FileStoreRepositoryTests()
        {
            _contextFactory = new TestDbContextFactory("FileStoreRepositoryTests");
            _fileStoreRepository = new FileStoreRepository(_contextFactory);
        }

        protected void InsertFileDescriptions(params FileDescription[] fileDescriptions)
        {
            var context = _contextFactory.CreateDbContext();
            context.FileDescriptions.AddRange(fileDescriptions);
            context.SaveChanges();
        }
    }

    public class GetSingleFileDescriptionAsync : FileStoreRepositoryTests
    {
        [Fact]
        public async Task ReturnsCorrectFileDescription()
        {
            // Arrange
            FileDescription _fileDescription1 = new()
            {
                StreamId = Guid.NewGuid(),
                FileName = "Test1",
                Description = "Test"
            };

            FileDescription _fileDescription2 = new()
            {
                StreamId = Guid.NewGuid(),
                FileName = "Test2",
                Description = "Test"
            };

            InsertFileDescriptions(_fileDescription1, _fileDescription2);

            // Act
            var result = await FileStoreRepository.GetSingleFileDescriptionAsync(_fileDescription1.StreamId);
            
            // Assert
            result.FileName.Should().Be(_fileDescription1.FileName);
        }

        [Fact]
        public async Task ReturnsNull_WhenNoFileDescriptionExists()
        {
            // Arrange
            var streamId = Guid.Empty;

            // Act
            var result = await FileStoreRepository.GetSingleFileDescriptionAsync(streamId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ThrowsDocumentStoreException_WhenErrorOccurs()
        {
            // Arrange
            var message = "test";
            var mockFactory = new Mock<IDbContextFactory<FileStoreDBContext>>();
            mockFactory
                .Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(message));
            var fileStoreRepository = new FileStoreRepository(mockFactory.Object);

            // Act
            var exception = fileStoreRepository.GetSingleFileDescriptionAsync(Guid.Empty).Exception;

            // Assert
            exception.InnerException.Should().BeOfType(typeof(DocumentStoreException));
            exception.InnerException.InnerException.Message.Should().Be(message);
        }
    }
}