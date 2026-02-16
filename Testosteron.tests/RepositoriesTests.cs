using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testosteron.Data;
using Testosteron.Domain.Enities;
using Testosteron.Domain.Repositories;

namespace Testosteron.tests;

[TestClass]
public class RepositoriesTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=testosteron_app_tests;User Id=postgres;Password=26569";

    [TestClass]
    public class TestRepositoryTests
    {
        private static ApplicationDbContext _sharedContext = null!;
        private static readonly List<Test> _createdTests = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            _sharedContext = new ApplicationDbContext(options);
            _sharedContext.Database.EnsureDeleted();
            _sharedContext.Database.EnsureCreated();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            foreach (var test in _createdTests)
            {
                var existing = _sharedContext.Tests.Find(test.Id);
                if (existing != null)
                {
                    _sharedContext.Tests.Remove(existing);
                }
            }
            _sharedContext.SaveChanges();
            _sharedContext.Dispose();
        }

        private ApplicationDbContext CreateContext()
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);
        }

        private TestRepository CreateRepository(ApplicationDbContext context) => new(context);

        private static Test CreateValidTest(string? title = "Test Title") => new()
        {
            Id = Guid.NewGuid(),
            Title = title ?? "Default Title",
            Description = "Test Description",
            TestFields = new List<TestField>
            {
                new() { Title = "Field 1", TestFieldType = "text", Required = false, Description = "Description", Options = Array.Empty<string>() }
            }
        };

        [TestCleanup]
        public void TestCleanup()
        {
            //using var cleanupContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            //    .UseNpgsql(ConnectionString)
            //    .Options);

            //foreach (var test in _createdTests)
            //{
            //    var existing = cleanupContext.Tests.Find(test.Id);
            //    if (existing != null)
            //    {
            //        cleanupContext.Tests.Remove(existing);
            //    }
            //}
            //cleanupContext.SaveChanges();
            //_createdTests.Clear();

            _sharedContext.Database.EnsureDeleted();
            _sharedContext.Database.EnsureCreated();
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            context.Tests.Add(test);
            await context.SaveChangesAsync();
            _createdTests.Add(test);

            var result = await repository.GetByIdAsync(test.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(test.Id, result.Id);
            Assert.AreEqual(test.Title, result.Title);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var result = await repository.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsEmpty_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var result = await repository.GetByIdAsync(Guid.Empty);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllAsync_WhenEntitiesExist_ReturnsAll()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test1 = CreateValidTest("Title 1");
            var test2 = CreateValidTest("Title 2");
            var test3 = CreateValidTest("Title 3");
            context.Tests.AddRange(test1, test2, test3);
            await context.SaveChangesAsync();
            _createdTests.AddRange(new[] { test1, test2, test3 });

            var result = (await repository.GetAllAsync()).ToList();

            Assert.HasCount(3, result);
        }

        [TestMethod]
        public async Task GetAllAsync_WhenNoEntities_ReturnsEmptyCollection()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var result = (await repository.GetAllAsync()).ToList();

            Assert.HasCount(0, result);
        }

        [TestMethod]
        public async Task FindAsync_WithMatchingPredicate_ReturnsMatchingEntities()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test1 = CreateValidTest("Matching Title");
            var test2 = CreateValidTest("Other Title");
            context.Tests.AddRange(test1, test2);
            await context.SaveChangesAsync();
            _createdTests.AddRange(new[] { test1, test2 });

            var result = (await repository.FindAsync(t => t.Title == "Matching Title")).ToList();

            Assert.HasCount(1, result);
            Assert.AreEqual("Matching Title", result[0].Title);
        }

        [TestMethod]
        public async Task FindAsync_WithNoMatchingPredicate_ReturnsEmptyCollection()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            context.Tests.Add(test);
            await context.SaveChangesAsync();
            _createdTests.Add(test);

            var result = (await repository.FindAsync(t => t.Title == "NonExistent")).ToList();

            Assert.HasCount(0, result);
        }

        [TestMethod]
        public async Task FindAsync_WithComplexPredicate_ReturnsCorrectSubset()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test1 = CreateValidTest("Alpha");
            var test2 = CreateValidTest("Beta");
            var test3 = CreateValidTest("Alpha");
            context.Tests.AddRange(test1, test2, test3);
            await context.SaveChangesAsync();
            _createdTests.AddRange(new[] { test1, test2, test3 });

            var result = (await repository.FindAsync(t => t.Title.StartsWith("A"))).ToList();

            Assert.HasCount(2, result);
            Assert.IsTrue(result.All(t => t.Title.StartsWith("A")));
        }

        [TestMethod]
        public async Task AddAsync_WithValidData_ReturnsEntityWithGeneratedId()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            test.Id = Guid.Empty;

            var result = await repository.AddAsync(test);
            await repository.SaveChangesAsync();
            _createdTests.Add(test);

            Assert.AreNotEqual(Guid.Empty, result.Id);
            Assert.AreEqual(test.Title, result.Title);
        }

        [TestMethod]
        public async Task AddAsync_WithNullEntity_ThrowsArgumentNullException()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.AddAsync(null!));
        }

        [TestMethod]
        public async Task AddAsync_WithEmptyTitle_SavesSuccessfully()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            test.Title = string.Empty;

            await repository.AddAsync(test);
            var affected = await repository.SaveChangesAsync();

            Assert.AreEqual(1, affected);
        }

        [TestMethod]
        public async Task UpdateAsync_WithValidData_MarksEntityAsModified()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            context.Tests.Add(test);
            await context.SaveChangesAsync();
            _createdTests.Add(test);

            test.Title = "Updated Title";
            await repository.UpdateAsync(test);
            var affected = await repository.SaveChangesAsync();

            Assert.AreEqual(1, affected);
            var updated = await repository.GetByIdAsync(test.Id);
            Assert.AreEqual("Updated Title", updated!.Title);
        }

        [TestMethod]
        public async Task UpdateAsync_WithNonExistentEntity_ThrowsConcurrencyException()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            test.Id = Guid.NewGuid();

            await repository.UpdateAsync(test);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await repository.SaveChangesAsync());
        }

        [TestMethod]
        public async Task UpdateAsync_WithEmptyTitle_SavesSuccessfully()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            context.Tests.Add(test);
            await context.SaveChangesAsync();
            _createdTests.Add(test);

            test.Title = "";
            await repository.UpdateAsync(test);
            var affected = await repository.SaveChangesAsync();

            Assert.AreEqual(1, affected);
        }

        [TestMethod]
        public async Task DeleteAsync_WithExistingEntity_RemovesEntity()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            context.Tests.Add(test);
            await context.SaveChangesAsync();
            _createdTests.Add(test);

            await repository.DeleteAsync(test);
            var affected = await repository.SaveChangesAsync();

            Assert.AreEqual(1, affected);
            Assert.IsNull(await repository.GetByIdAsync(test.Id));
        }

        [TestMethod]
        public async Task DeleteAsync_WithNonExistentEntity_ThrowsConcurrencyException()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();
            test.Id = Guid.NewGuid();

            await repository.DeleteAsync(test);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await repository.SaveChangesAsync());
        }

        [TestMethod]
        public async Task SaveChangesAsync_AfterAdd_ReturnsPositive()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var test = CreateValidTest();

            await repository.AddAsync(test);
            var affected = await repository.SaveChangesAsync();
            _createdTests.Add(test);

            Assert.AreEqual(1, affected);
        }

        [TestMethod]
        public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var affected = await repository.SaveChangesAsync();

            Assert.AreEqual(0, affected);
        }
    }
}
