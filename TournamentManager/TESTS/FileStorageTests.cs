using NUnit.Framework;
using System.IO;
using TournemantManager.Infrastructure;
using TournemantManager.Core.Models;

namespace TournemantManager.TESTS;

[TestFixture]
public class FileStorageTests
{
    private string _testFilePath = "test_data.json";

    [Test]
    public void SaveAndLoad_ValidData_ShouldPreserveDataCorrectly()
    {
        var storage = new FileStorage<Team>(_testFilePath);
        var team = new Team { Id = 99, Name = "TestStorageTeam" };

        try
        {
            storage.Save(team);
            var loadedTeam = storage.Load();

            Assert.That(loadedTeam, Is.Not.Null);
            Assert.That(loadedTeam.Id, Is.EqualTo(99));
            Assert.That(loadedTeam.Name, Is.EqualTo("TestStorageTeam"));
        }
        finally
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }

    [Test]
    public void Load_FileDoesNotExist_ShouldReturnNull()
    {
        var storage = new FileStorage<Team>("non_existent.json");
        var loadedData = storage.Load();
        Assert.That(loadedData, Is.Null);
    }

    [Test]
    public void FileStorage_SaveToInvalidDrive_ShouldThrowDirectoryNotFoundException()
    {
        string impossiblePath = "Z:\\non_existent_folder\\data.json";
        var storage = new FileStorage<Team>(impossiblePath);
        var team = new Team { Id = 1, Name = "Test" };

        Assert.Throws<DirectoryNotFoundException>(() => storage.Save(team));
    }
}