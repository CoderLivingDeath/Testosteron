using Testosteron.Models;
using System.ComponentModel.DataAnnotations;
using Testosteron.Data;
using Testosteron.Domain.Enities;
using System.Threading.Tasks;

namespace Testosteron.Services
{
    public class TestManager
    {
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<Answers> _answersRepository;

        public TestManager(IRepository<Test> testRepository, IRepository<Answers> answersRepository)
        {
            _testRepository = testRepository;
            _answersRepository = answersRepository;
        }

        public async Task<Result<Test?>> AddNewTestAsync(CreateTestDTO dto)
        {
            try
            {
                var result = await _testRepository.AddAsync(dto);

                if (result == null) return Result<Test?>.CreateFailure(default, "Test not found");

                await _testRepository.SaveChangesAsync();
                return Result<Test?>.CreateSuccess(result, "Test was created");
            }
            catch (Exception ex)
            {
                return Result<Test?>.CreateFailure(default, "Server error").ErrorsFromException(ex);
            }
        }

        public async Task<Result> DeleteTestAsync(DeleteTestDTO dto)
        {
            try
            {
                var entry = await _testRepository.GetByIdAsync(dto.Id);

                if (entry == null) return Result.CreateFailure("Test not found");

                await _testRepository.DeleteAsync(entry);
                await _testRepository.SaveChangesAsync();

                return Result.CreateSuccess($"Test with id: {dto.Id} deleted");
            }
            catch (Exception ex)
            {
                return Result.CreateFailure("Server error").ErrorsFromException(ex);
            }
        }

        public async Task<Result<Answers?>> AddAnswersToTest(AddAnswersDTO dto)
        {
            try
            {
                var answers = new Answers
                {
                    Id = Guid.NewGuid(),
                    TestId = dto.TestId,
                    UserId = dto.UserId,
                    Content = dto.Content
                };

                await _answersRepository.AddAsync(answers);
                await _answersRepository.SaveChangesAsync();

                return Result<Answers?>.CreateSuccess(answers, "Answers added");
            }
            catch (Exception ex)
            {
                return Result<Answers?>.CreateFailure(default, "Server error").ErrorsFromException(ex);
            }
        }

        public async Task<Result<IEnumerable<Test>>> GetTests()
        {
            try
            {
                var tests = await _testRepository.GetAllAsync();
                if (!tests.Any()) return Result<IEnumerable<Test>>.CreateFailure(tests, "Not found");

                return Result<IEnumerable<Test>>.CreateSuccess(tests, "Tests retrieved");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Test>>.CreateFailure(default!, "Server error").ErrorsFromException(ex);
            }
        }

        public async Task<Result<Test?>> GetTestByIdAsync(Guid id)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(id);

                if (test == null) return Result<Test?>.CreateFailure(default, $"Test not found");

                return Result<Test?>.CreateSuccess(test, "Test retrieved");
            }
            catch (Exception ex)
            {
                return Result<Test?>.CreateFailure(default, "Server error").ErrorsFromException(ex);
            }
        }

        public async Task<Result<Answers?>> GetUserAnswersForTest(Guid userId, Guid testId)
        {
            try
            {
                var answers = await _answersRepository.FindAsync(item => item.TestId == testId && item.UserId == userId);

                var answer = answers.FirstOrDefault();
                if (answer == null) return Result<Answers?>.CreateFailure(default, "Not found");

                return Result<Answers?>.CreateSuccess(answer, "Answers retrieved");
            }
            catch (Exception ex)
            {
                return Result<Answers?>.CreateFailure(default, "Server error").ErrorsFromException(ex);
            }
        }

        public async Task<Result> UpdateTestAsync(UpdateTestDTO dto)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(dto.Id);
                if (test == null) return Result.CreateFailure("Test not found");

                test.Title = dto.Title;
                test.Description = dto.Description;
                test.TestFields = dto.TestFields;

                await _testRepository.SaveChangesAsync();
                return Result.CreateSuccess("Test updated successfully");
            }
            catch (Exception ex)
            {
                return Result.CreateFailure("Server error").ErrorsFromException(ex);
            }
        }
    }

    public class UpdateTestDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(600)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public List<TestField> TestFields { get; set; } = new();

        public static implicit operator Test(UpdateTestDTO dto)
        {
            return new Test
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                TestFields = dto.TestFields
            };
        }
    }

    public class AddAnswersDTO
    {
        [Required]
        public Guid TestId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        public List<FieldAnswer> Content { get; set; } = new();

        public static implicit operator Answers(AddAnswersDTO dto)
        {
            return new Answers()
            {
                Id = Guid.Empty,
                UserId = dto.UserId,
                Content = dto.Content
            };
        }
    }

    public class DeleteTestDTO
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class CreateTestDTO
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(600)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public List<TestField> TestFields { get; set; } = new();

        public static implicit operator Test(CreateTestDTO dto)
        {
            return new Test
            {
                Id = Guid.Empty,
                Title = dto.Title,
                Description = dto.Description,
                TestFields = dto.TestFields
            };
        }
    }
}
