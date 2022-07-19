using Attrecto.Controllers;
using Attrecto.IdentityServer;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AttrectoUnitTests
{
    public class ImageControllerTests
    {
        private Mock<IClaimsHelper> claimHelperMock;

        [SetUp]
        public void Setup()
        {
            claimHelperMock = new Mock<IClaimsHelper>();
        }

        [Test]
        public async Task UploadImage_WithoutFile_ReturnsBadRequest()
        {
            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);

            var controller = new ImageController(claimHelperMock.Object);
            var result = await controller.UploadImage(null);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetImage_NotExistingFile_ReturnsBadRequest()
        {
            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(100);

            var controller = new ImageController(claimHelperMock.Object);
            var result = await controller.GetImage();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UploadImageSuccess_ThanReturnRecentlyCreated()
        {
            //have to run these sequaly, otherwise getimage has nothing to return
            await UploadImage_Success();
            await GetImage_Success();
        }

        public async Task UploadImage_Success()
        {
            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);

            string testImageFolder = Path.Combine(Environment.CurrentDirectory, "Images");
            string filePath = Path.Combine(testImageFolder, $"test_image_to_upload.jpg");
            var content = await System.IO.File.ReadAllBytesAsync(filePath);

            IFormFile file = new FormFile(new MemoryStream(content), 0, content.Length, "test_image_to_upload.jpg", "test_image_to_upload.jpg");
            var controller = new ImageController(claimHelperMock.Object);
            var result = await controller.UploadImage(file);
            result.Should().BeOfType<NoContentResult>();
        }

        public async Task GetImage_Success()
        {
            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);

            var controller = new ImageController(claimHelperMock.Object);
            var result = await controller.GetImage();
            result.Should().BeOfType<FileContentResult>();
        }
    }
}
