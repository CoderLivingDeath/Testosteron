using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testosteron.Data;
using Testosteron.Domain.Enities;
using Testosteron.Domain.Repositories;
using Testosteron.Models;
using Testosteron.Services;

namespace Testosteron.tests;

[TestClass]
public class TestManagerTests
{
    private Mock<IRepository<Test>> _testRepositoryMock = null!;
    private Mock<IRepository<Answers>> _answersRepositoryMock = null!;
    private TestManager _testManager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _testRepositoryMock = new Mock<IRepository<Test>>();
        _answersRepositoryMock = new Mock<IRepository<Answers>>();
        _testManager = new TestManager(_testRepositoryMock.Object, _answersRepositoryMock.Object);
    }

    #region AddNewTestAsync Tests

    [TestMethod]
    public async Task AddNewTestAsync_WithValidDTO_ReturnsSuccess()
    {
        var dto = CreateValidCreateTestDTO();
        var createdTest = (Test)dto;
        _testRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Test>())).ReturnsAsync(createdTest);
        _testRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _testManager.AddNewTestAsync(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Test was created", result.Message);
        Assert.IsNotNull(result.Value);
        _testRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Test>()), Times.Once);
        _testRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task AddNewTestAsync_WhenRepositoryReturnsNull_ReturnsFailure()
    {
        var dto = CreateValidCreateTestDTO();
        _testRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Test>())).ReturnsAsync((Test)null!);

        var result = await _testManager.AddNewTestAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Test not found", result.Message);
    }

    [TestMethod]
    public async Task AddNewTestAsync_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        var dto = CreateValidCreateTestDTO();
        _testRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Test>())).ThrowsAsync(new Exception("Database error"));

        var result = await _testManager.AddNewTestAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(1, result.Errors.Length);
        Assert.AreEqual("Database error", result.Errors[0]);
    }

    #endregion

    #region DeleteTestAsync Tests

    [TestMethod]
    public async Task DeleteTestAsync_WhenTestExists_ReturnsSuccess()
    {
        var testId = Guid.NewGuid();
        var dto = new DeleteTestDTO { Id = testId };
        var test = CreateValidTest();
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(test);
        _testRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _testManager.DeleteTestAsync(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual($"Test with id: {testId} deleted", result.Message);
        _testRepositoryMock.Verify(r => r.DeleteAsync(test), Times.Once);
    }

    [TestMethod]
    public async Task DeleteTestAsync_WhenTestNotFound_ReturnsFailure()
    {
        var testId = Guid.NewGuid();
        var dto = new DeleteTestDTO { Id = testId };
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync((Test)null!);

        var result = await _testManager.DeleteTestAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Test not found", result.Message);
    }

    [TestMethod]
    public async Task DeleteTestAsync_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        var testId = Guid.NewGuid();
        var dto = new DeleteTestDTO { Id = testId };
        var test = CreateValidTest();
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(test);
        _testRepositoryMock.Setup(r => r.DeleteAsync(test)).ThrowsAsync(new Exception("Delete failed"));

        var result = await _testManager.DeleteTestAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
    }

    #endregion

    #region AddAnswersToTest Tests

    [TestMethod]
    public async Task AddAnswersToTest_WithValidDTO_ReturnsSuccess()
    {
        var dto = CreateValidAddAnswersDTO();
        var createdAnswers = (Answers)dto;
        _answersRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Answers>())).ReturnsAsync(createdAnswers);
        _answersRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _testManager.AddAnswersToTest(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Answers added", result.Message);
        Assert.IsNotNull(result.Value);
        _answersRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Answers>()), Times.Once);
    }

    [TestMethod]
    public async Task AddAnswersToTest_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        var dto = CreateValidAddAnswersDTO();
        _answersRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Answers>())).ThrowsAsync(new Exception("Save failed"));

        var result = await _testManager.AddAnswersToTest(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
    }

    #endregion

    #region GetTests Tests

    [TestMethod]
    public async Task GetTests_WhenTestsExist_ReturnsSuccessWithTests()
    {
        var tests = new[] { CreateValidTest(), CreateValidTest("Test 2") };
        _testRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tests);

        var result = await _testManager.GetTests();

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Tests retrieved", result.Message);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count());
    }

    [TestMethod]
    public async Task GetTests_WhenNoTestsExist_ReturnsFailure()
    {
        _testRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Test>());

        var result = await _testManager.GetTests();

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Not found", result.Message);
    }

    [TestMethod]
    public async Task GetTests_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        _testRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        var result = await _testManager.GetTests();

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
    }

    #endregion

    #region GetTestByIdAsync Tests

    [TestMethod]
    public async Task GetTestByIdAsync_WhenTestExists_ReturnsSuccess()
    {
        var testId = Guid.NewGuid();
        var test = CreateValidTest();
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(test);

        var result = await _testManager.GetTestByIdAsync(testId);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Test retrieved", result.Message);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(test.Id, result.Value.Id);
    }

    [TestMethod]
    public async Task GetTestByIdAsync_WhenTestNotFound_ReturnsFailure()
    {
        var testId = Guid.NewGuid();
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync((Test)null!);

        var result = await _testManager.GetTestByIdAsync(testId);

        Assert.IsFalse(result.Success);
        Assert.AreEqual($"Test not found", result.Message);
    }

    [TestMethod]
    public async Task GetTestByIdAsync_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        var testId = Guid.NewGuid();
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ThrowsAsync(new Exception("Database error"));

        var result = await _testManager.GetTestByIdAsync(testId);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
    }

    #endregion

    #region GetUserAnswersForTest Tests

    [TestMethod]
    public async Task GetUserAnswersForTest_WhenAnswersExist_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        var answers = new Answers { Id = Guid.NewGuid(), UserId = userId, TestId = testId };
        _answersRepositoryMock.Setup(r => r.FindAsync(a => a.TestId == testId && a.UserId == userId))
            .ReturnsAsync(new[] { answers });

        var result = await _testManager.GetUserAnswersForTest(userId, testId);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Answers retrieved", result.Message);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(userId, result.Value.UserId);
    }

    [TestMethod]
    public async Task GetUserAnswersForTest_WhenNotFound_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        _answersRepositoryMock.Setup(r => r.FindAsync(a => a.TestId == testId && a.UserId == userId))
            .ReturnsAsync(Enumerable.Empty<Answers>());

        var result = await _testManager.GetUserAnswersForTest(userId, testId);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Not found", result.Message);
    }

    [TestMethod]
    public async Task GetUserAnswersForTest_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        var userId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        _answersRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Answers, bool>>>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _testManager.GetUserAnswersForTest(userId, testId);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
    }

    #endregion

    #region UpdateTestAsync Tests

    [TestMethod]
    public async Task UpdateTestAsync_WhenTestExists_ReturnsSuccess()
    {
        var testId = Guid.NewGuid();
        var existingTest = CreateValidTest();
        var dto = CreateValidUpdateTestDTO(testId);
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(existingTest);
        _testRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _testManager.UpdateTestAsync(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Test updated successfully", result.Message);
        _testRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateTestAsync_WhenTestNotFound_ReturnsFailure()
    {
        var testId = Guid.NewGuid();
        var dto = CreateValidUpdateTestDTO(testId);
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync((Test)null!);

        var result = await _testManager.UpdateTestAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Test not found", result.Message);
    }

    [TestMethod]
    public async Task UpdateTestAsync_WhenExceptionOccurs_ReturnsFailureWithError()
    {
        var testId = Guid.NewGuid();
        var existingTest = CreateValidTest();
        var dto = CreateValidUpdateTestDTO(testId);
        _testRepositoryMock.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(existingTest);
        _testRepositoryMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Update failed"));

        var result = await _testManager.UpdateTestAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Server error", result.Message);
    }

    #endregion

    #region DTO Implicit Conversion Tests

    [TestMethod]
    public void UpdateTestDTO_ImplicitConversion_ToTest_MapsCorrectly()
    {
        var testId = Guid.NewGuid();
        var dto = new UpdateTestDTO
        {
            Id = testId,
            Title = "Updated Title",
            Description = "Updated Description",
            TestFields = new List<TestField>
            {
                new() { Title = "Field 1", TestFieldType = "text" }
            }
        };

        Test test = dto;

        Assert.AreEqual(testId, test.Id);
        Assert.AreEqual("Updated Title", test.Title);
        Assert.AreEqual("Updated Description", test.Description);
        Assert.HasCount(1, test.TestFields);
    }

    [TestMethod]
    public void AddAnswersDTO_ImplicitConversion_ToAnswers_MapsCorrectly()
    {
        var testId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new AddAnswersDTO
        {
            TestId = testId,
            UserId = userId,
            Content = new List<FieldAnswer>
            {
                new() { }
            }
        };

        Answers answers = dto;

        Assert.AreNotEqual(Guid.Empty, answers.Id);
        Assert.AreEqual(testId, answers.TestId);
        Assert.AreEqual(userId, answers.UserId);
        Assert.HasCount(1, answers.Content.Content);
    }

    [TestMethod]
    public void CreateTestDTO_ImplicitConversion_ToTest_MapsCorrectly()
    {
        var dto = new CreateTestDTO
        {
            Title = "New Test",
            Description = "Test Description",
            TestFields = new List<TestField>
            {
                new() { Title = "Field 1", TestFieldType = "text" }
            }
        };

        Test test = dto;

        Assert.AreEqual(Guid.Empty, test.Id);
        Assert.AreEqual("New Test", test.Title);
        Assert.AreEqual("Test Description", test.Description);
        Assert.AreEqual(1, test.TestFields.Count);
    }

    #endregion

    #region Helper Methods

    private static CreateTestDTO CreateValidCreateTestDTO()
    {
        return new CreateTestDTO
        {
            Title = "Test Title",
            Description = "Test Description",
            TestFields = new List<TestField>
            {
                new() { Title = "Field 1", TestFieldType = "text", Required = false }
            }
        };
    }

    private static AddAnswersDTO CreateValidAddAnswersDTO()
    {
        return new AddAnswersDTO
        {
            TestId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = new List<FieldAnswer>
            {
                new() { TextValue = [],FieldType ="text",   }
            }
        };
    }

    private static UpdateTestDTO CreateValidUpdateTestDTO(Guid id)
    {
        return new UpdateTestDTO
        {
            Id = id,
            Title = "Updated Title",
            Description = "Updated Description",
            TestFields = new List<TestField>
            {
                new() { Title = "Updated Field", TestFieldType = "text" }
            }
        };
    }

    private static Test CreateValidTest(string? title = "Test Title")
    {
        return new Test
        {
            Id = Guid.NewGuid(),
            Title = title ?? "Default Title",
            Description = "Test Description",
            TestFields = new List<TestField>
            {
                new() { Title = "Field 1", TestFieldType = "text", Required = false }
            }
        };
    }

    #endregion
}
