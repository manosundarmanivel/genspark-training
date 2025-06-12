using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using ElearnAPI.Controllers;
using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace ElearnAPI.Tests.Controllers
{
    [TestFixture]
    public class UploadControllerTests
    {
        private Mock<IUploadService> _uploadServiceMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IClientProxy> _clientProxyMock;
        private UploadController _controller;

        [SetUp]
        public void SetUp()
        {
            _uploadServiceMock = new Mock<IUploadService>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            _controller = new UploadController(_uploadServiceMock.Object, _hubContextMock.Object);
        }

        [Test]
public async Task Upload_ValidFile_ReturnsSuccess()
{
    // Arrange
    var fileName = "test.pdf";
    var courseId = Guid.NewGuid();
    var formFileMock = new Mock<IFormFile>();
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("dummy file content");
    writer.Flush();
    stream.Position = 0;

    formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
    formFileMock.Setup(f => f.FileName).Returns(fileName);
    formFileMock.Setup(f => f.Length).Returns(stream.Length);
    formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken ct) =>
    {
        stream.CopyTo(target);
        return Task.CompletedTask;
    });

    var dto = new UploadFileDto
    {
        File = formFileMock.Object,
        CourseId = courseId
    };

    var uploadedFileDto = new UploadedFileDto
    {
        FileName = fileName,
        Path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName),
        CourseId = courseId
    };

    _uploadServiceMock
        .Setup(s => s.UploadFileAsync(It.IsAny<UploadedFileDto>()))
        .ReturnsAsync(uploadedFileDto);

    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    // Act
    var result = await _controller.Upload(dto) as OkObjectResult;

    // Assert
    Assert.IsNotNull(result);
    
    // Convert anonymous object to dictionary
    var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

    Assert.That(dict["success"].ToString(), Is.EqualTo("True"));

    var dataJson = dict["data"].ToString();
    Assert.IsTrue(dataJson!.Contains(fileName));
}

    }
}
