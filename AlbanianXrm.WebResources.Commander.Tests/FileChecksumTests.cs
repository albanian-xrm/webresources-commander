using System;
using System.IO;
using Xunit;

namespace AlbanianXrm.WebResources
{
    public class FileChecksumTests
    {
        [Fact]
        public void FilesWithSameContentReturnTheSameChecksum()
        {
            // Arrange
            var executingFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var file1 = Path.Combine(executingFolder, "TestFiles/Scenario01/albx_/js/account.events.js");
            var file2 = Path.Combine(executingFolder, "TestFiles/Scenario02/albx_/js/account.events.js");
     
            // Act
            var checksum1 = FileChecksum.GetSHA1Checksum(file1);
            var checksum2 = FileChecksum.GetSHA1Checksum(file2);

            // Assert
            Assert.Equal(checksum1, checksum2);
        }

        [Fact]
        public void FilesWithDifferentContentReturnDifferentChecksums()
        {
            // Arrange
            var executingFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var file1 = Path.Combine(executingFolder, "TestFiles/Scenario01/albx_/js/exclude.js");
            var file2 = Path.Combine(executingFolder, "TestFiles/Scenario02/albx_/js/account.events.js");

            // Act
            var checksum1 = FileChecksum.GetSHA1Checksum(file1);
            var checksum2 = FileChecksum.GetSHA1Checksum(file2);

            // Assert
            Assert.NotEqual(checksum1, checksum2);
        }
    }
}
