using ElearnAPI.Data;
using ElearnAPI.Models;
using ElearnAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElearnAPI.Tests.Repositories
{
    public class UploadRepositoryTests
    {
        private ElearnDbContext _context = null!;
        private UploadRepository _repository = null!;
        private Guid _testCourseId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ElearnDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ElearnDbContext(options);
            _repository = new UploadRepository(_context);
            _testCourseId = Guid.NewGuid();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

[Test]
public async Task AddAsync_ShouldAddFileSuccessfully()
{
    var file = new UploadedFile
    {
        FileName = "test.pdf",
        FileType = "application/pdf",
        CourseId = _testCourseId,
        Path = "/uploads/test.pdf",
        Topic = "Test Topic"
    };

    await _repository.AddAsync(file);

    var result = await _context.UploadedFiles.FindAsync(file.Id);
    Assert.That(result, Is.Not.Null);
    Assert.That(result!.FileName, Is.EqualTo("test.pdf"));
}


  [Test]
public async Task DeleteAsync_ShouldRemoveFileSuccessfully()
{
    var file = new UploadedFile
    {
        FileName = "delete.pdf",
        FileType = "application/pdf",
        CourseId = _testCourseId,
        Path = "/uploads/delete.pdf",
        Topic = "Delete Topic"
    };

    await _context.UploadedFiles.AddAsync(file);
    await _context.SaveChangesAsync();

    await _repository.DeleteAsync(file);

    var result = await _context.UploadedFiles.FirstOrDefaultAsync(f => f.Id == file.Id);
    Assert.That(result, Is.Null);
}

[Test]
public async Task GetByIdAsync_ShouldReturnCorrectFile()
{
    var file = new UploadedFile
    {
        FileName = "findme.pdf",
        FileType = "application/pdf",
        CourseId = _testCourseId,
        Path = "/uploads/findme.pdf",
        Topic = "FindMe Topic"
    };

    await _context.UploadedFiles.AddAsync(file);
    await _context.SaveChangesAsync();

    var result = await _repository.GetByIdAsync(file.Id);
    Assert.That(result, Is.Not.Null);
    Assert.That(result!.FileName, Is.EqualTo("findme.pdf"));
}

[Test]
public async Task GetFilesByCourseIdAsync_ShouldReturnCorrectFiles()
{
    var file1 = new UploadedFile
    {
        FileName = "c1file1.pdf",
        FileType = "application/pdf",
        CourseId = _testCourseId,
        Path = "/uploads/c1file1.pdf",
        Topic = "Course Topic 1" 
    };

    var file2 = new UploadedFile
    {
        FileName = "c1file2.pdf",
        FileType = "application/pdf",
        CourseId = _testCourseId,
        Path = "/uploads/c1file2.pdf",
        Topic = "Course Topic 2" 
    };

    var otherCourseFile = new UploadedFile
    {
        FileName = "othercourse.pdf",
        FileType = "application/pdf",
        CourseId = Guid.NewGuid(),
        Path = "/uploads/othercourse.pdf",
        Topic = "Other Course Topic" 
    };

    await _context.UploadedFiles.AddRangeAsync(file1, file2, otherCourseFile);
    await _context.SaveChangesAsync();

    var result = await _repository.GetFilesByCourseIdAsync(_testCourseId);

    Assert.That(result.Count, Is.EqualTo(2));
    Assert.That(result.Any(f => f.FileName == "c1file1.pdf"), Is.True);
    Assert.That(result.Any(f => f.FileName == "c1file2.pdf"), Is.True);
}

    }
}
