using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HoangCN.Core.Common.Exceptions;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HoangCN.MainSystem.Tests
{
    public class FileServiceTests : IDisposable
    {
        private readonly FakeReadDL _fakeReadDL;
        private readonly FakeWriteDL _fakeWriteDL;
        private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ILogger<FileService>> _mockLogger;
        private readonly FileService _fileService;
        private readonly string _tempWebRoot;

        public FileServiceTests()
        {
            _fakeReadDL = new FakeReadDL();
            _fakeWriteDL = new FakeWriteDL();
            _mockHttpAccessor = new Mock<IHttpContextAccessor>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<FileService>>();

            // Setup temporary upload folder
            _tempWebRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(_tempWebRoot, "upload"));
            _mockEnv.Setup(e => e.WebRootPath).Returns(_tempWebRoot);

            // Mock HttpContext for audit fields with a default authenticated user
            var mockContext = new Mock<HttpContext>();
            var claims = new List<System.Security.Claims.Claim> { new System.Security.Claims.Claim("UserName", "TestUser") };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuth");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            mockContext.Setup(c => c.User).Returns(principal);
            _mockHttpAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

            _fileService = new FileService(
                _fakeReadDL,
                _fakeWriteDL,
                _mockHttpAccessor.Object,
                _mockEnv.Object,
                _mockLogger.Object
            );
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempWebRoot))
            {
                Directory.Delete(_tempWebRoot, true);
            }
        }

        [Fact]
        public async Task SaveFileAsync_ShouldThrowBadRequestException_WhenFileIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _fileService.SaveFileAsync(null!));
        }

        [Fact]
        public async Task SaveFileAsync_ShouldSaveFilePhysicallyAndSaveRecordToDatabase_WhenFileIsValid()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var content = "Hello World from Unit Tests";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            mockFile.Setup(f => f.Length).Returns(ms.Length);
            mockFile.Setup(f => f.FileName).Returns("test.txt");
            mockFile.Setup(f => f.ContentType).Returns("text/plain");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<Stream, System.Threading.CancellationToken>((stream, token) => ms.CopyTo(stream))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _fileService.SaveFileAsync(mockFile.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.txt", result.OriginName);
            Assert.Equal("text/plain", result.Type);
            Assert.Equal(ms.Length, result.Size);
            Assert.True(File.Exists(Path.Combine(_tempWebRoot, "upload", result.Url.Replace("/upload/", ""))));

            Assert.Contains(result, _fakeWriteDL.InsertedEntities.Cast<ResourceFile>());
        }

        [Fact]
        public async Task GetFileUrlAsync_ShouldThrowArgumentException_WhenFileIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _fileService.GetFileUrlAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetFileUrlAsync_ShouldThrowNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _fileService.GetFileUrlAsync(fileId));
        }

        [Fact]
        public async Task GetFileUrlAsync_ShouldReturnUrl_WhenFileExists()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var resourceFile = new ResourceFile
            {
                ResourceFileId = fileId,
                OriginName = "test.txt",
                Type = "text/plain",
                Url = "/upload/test.txt"
            };

            _fakeReadDL.QueryResults.Add(resourceFile);

            // Act
            var url = await _fileService.GetFileUrlAsync(fileId);

            // Assert
            Assert.Equal("/upload/test.txt", url);
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldDeleteFilePhysicallyAndDatabaseRecord_WhenFileExists()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var uniqueFileName = $"{fileId}.txt";
            var filePath = Path.Combine(_tempWebRoot, "upload", uniqueFileName);
            File.WriteAllText(filePath, "temp content");

            var resourceFile = new ResourceFile
            {
                ResourceFileId = fileId,
                OriginName = "test.txt",
                Type = "text/plain",
                Url = $"/upload/{uniqueFileName}"
            };

            _fakeReadDL.QueryResults.Add(resourceFile);

            // Act
            await _fileService.DeleteFileAsync(fileId);

            // Assert
            Assert.False(File.Exists(filePath)); // Physical file deleted
            Assert.Contains(resourceFile, _fakeWriteDL.DeletedEntities.Cast<ResourceFile>());
        }
    }
}
