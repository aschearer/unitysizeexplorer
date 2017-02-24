using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using SpottedZebra.UnitySizeExplorer.WPF.ViewModels.Pages;

    /// <summary>
    /// Summary description for FileBuilderTest
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
            var result = FileBuilder.FromStream(stream);

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
            var result = FileBuilder.FromStream(stream);

            // assert
            var first = result.First();

            result.Should().HaveCount(2);
            first.Item1.Should().Be("Assets/Spritesheets/v2/Spritesheet1.png");
            first.Item2.Should().Be(21f);
        }
    }
    
}
