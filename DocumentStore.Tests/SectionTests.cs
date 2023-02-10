using Bunit;
using Bunit.TestDoubles;
using DocumentStore.Components;
using DocumentStore.Core.Dto;
using DocumentStore.Core.Enums;
using DocumentStore.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DocumentStore.Tests
{
    public class SectionTests : TestContext
    {
        private readonly Mock<IFileStoreService> _fileStoreServiceMock = new();

        public SectionTests() 
        {
            Services.AddSingleton(_fileStoreServiceMock.Object);
        }

        [Fact]
        public void ComponentRendersCorrectly_NonAdminRole()
        {
            // Arrange
            _fileStoreServiceMock
                .Setup(x => x.DeleteFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _fileStoreServiceMock
                .Setup(x => x.GetFileDescriptionsAsync(Sections.CompanyPolicies, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<FileDescriptionShortDto>() { new FileDescriptionShortDto() { FileName = "Test" } });
            
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TEST USER");

            // Act
            var cut = RenderComponent<Section>(parameters => parameters
                .Add(p => p.SectionName, Sections.CompanyPolicies)
            );

            // Assert
            var rows = cut.FindAll("table tr");
            rows.Should().HaveCount(2);
            rows[1].InnerHtml.Should().Contain("Test");

            var button = rows[1].GetElementsByTagName("button");
            button.Should().ContainSingle();
            button[0].TextContent.Should().Be("Download");
        }

        [Fact]
        public void ComponentRendersCorrectly_AdminRole()
        {
            // Arrange
            _fileStoreServiceMock
                .Setup(x => x.DeleteFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _fileStoreServiceMock
                .Setup(x => x.GetFileDescriptionsAsync(Sections.CompanyPolicies, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<FileDescriptionShortDto>() { new FileDescriptionShortDto() { FileName = "Test" } });

            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TEST USER");
            authContext.SetPolicies("RequireAdminRole");

            // Act
            var cut = RenderComponent<Section>(parameters => parameters
                .Add(p => p.SectionName, Sections.CompanyPolicies)
            );

            // Assert
            var rows = cut.FindAll("table tr");
            rows.Should().HaveCount(2);
            rows[1].InnerHtml.Should().Contain("Test");

            var button = rows[1].GetElementsByTagName("button");
            button.Should().HaveCount(2);
            button[0].TextContent.Should().Be("Download");
            button[1].TextContent.Should().Be("Delete");

            var anchor = rows[1].GetElementsByTagName("a");
            anchor.Should().ContainSingle();
            anchor[0].TextContent.Should().Be("Update");
        }
    }
}