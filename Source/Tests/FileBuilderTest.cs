namespace Tests
{
    using System.IO;
    using System.Linq;
    using System.Text;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpottedZebra.UnitySizeExplorer.WPF.ViewModels.Pages;

    /// <summary>
    ///     Summary description for FileBuilderTest
    /// </summary>
    [TestClass]
    public class FileBuilderTest
    {
        [TestMethod]
        public void LogWithoutStartWordShouldNotReturnAnyFiles()
        {
            // arrange
            var buffer = Encoding.UTF8.GetBytes(TestResources.LogWithoutStartWord);
            var stream = new MemoryStream(buffer);

            // act
            var task = FileBuilder.FromStream(stream);
            var result = task.Result;

            // assert
            result.Should().HaveCount(0);
        }

        [TestMethod]
        public void LogWithSingleBlockShouldReturnGivenInformation()
        {
            // arrange
            var buffer = Encoding.UTF8.GetBytes(TestResources.LogWithSingleBlockOfFiles);
            var stream = new MemoryStream(buffer);

            // act
            var task = FileBuilder.FromStream(stream);
            var result = task.Result;

            // assert
            var first = result.First();

            result.Should().HaveCount(2);
            first.Item1.Should().Be("file1");
            first.Item2.Should().Be(21f);
        }

        [TestMethod]
        public void LogWithMultipleBlocksShouldReturnGivenInformation()
        {
            // arrange
            var buffer = Encoding.UTF8.GetBytes(TestResources.LogWithMultipleBlocksOfFiles);
            var stream = new MemoryStream(buffer);

            // act
            var task = FileBuilder.FromStream(stream);
            var result = task.Result;

            // assert
            var first = result.First();

            result.Should().HaveCount(6);
            first.Item1.Should().Be("file1");
            first.Item2.Should().Be(21f);
        }
    }
}