using AutoMapper;
using DocumentStore.Core.Dto;
using DocumentStore.Core.Interfaces;
using DocumentStore.Core.Models;
using DocumentStore.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IO;

namespace DocumentStore.Services.Tests
{
    public abstract class FileStoreServiceTests
    {
        protected readonly Mock<IFileStoreRepository> FileStoreRepositoryMock = new();
        protected readonly Mock<IConfiguration> ConfigurationMock = new();
        protected readonly IMapper Mapper;
        protected readonly FileStoreService FileStoreService;

        public FileStoreServiceTests()
        {
            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            Mapper = mockMapper.CreateMapper();
            FileStoreService = new FileStoreService(FileStoreRepositoryMock.Object, ConfigurationMock.Object, Mapper);
        }
    }

    public class GetSingleFileDescriptionAsync : FileStoreServiceTests
    {
        [Fact]
        public async Task Returns_FileDescriptionDto()
        {
            // Arrange
            var streamId = Guid.NewGuid();
            var description = "test";
            var fileDescription = new FileDescription()
            {
                StreamId= streamId,
                Description= description
            };
            var fileDescriptionDto = new FileDescriptionDto();
            FileStoreRepositoryMock
                .Setup(x => x.GetSingleFileDescriptionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fileDescription);

            // Act
            var result = await FileStoreService.GetSingleFileDescriptionAsync(streamId);

            // Assert
            result.StreamId.Should().Be(streamId);
            result.Description.Should().Be(description);
        }

        [Fact]
        public async Task Calls_GetSingleFileDescriptionAsync_On_FileStoreRepository()
        {
            // Arrange
            var streamId = Guid.NewGuid();
            var fileDescription = new FileDescription();
            var fileDescriptionDto = new FileDescriptionDto();
            FileStoreRepositoryMock
                .Setup(x => x.GetSingleFileDescriptionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fileDescription);

            // Act
            await FileStoreService.GetSingleFileDescriptionAsync(streamId);

            // Assert
            FileStoreRepositoryMock.Verify(x => x.GetSingleFileDescriptionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ThrowsDocumentStoreException_WhenErrorOccurs()
        {
            // Arrange
            var message = "test";
            FileStoreRepositoryMock
                .Setup(x => x.GetSingleFileDescriptionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(message));

            // Act
            var exception = FileStoreService.GetSingleFileDescriptionAsync(Guid.NewGuid()).Exception;

            // Assert
            exception.InnerException.Should().BeOfType(typeof(DocumentStoreException));
            exception.InnerException.InnerException.Message.Should().Be(message);
        }
    }
}