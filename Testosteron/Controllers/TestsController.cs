using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Testosteron.Domain.Enities;
using Testosteron.Models;

namespace Testosteron.Controllers
{
    [Route("Tests")]
    public class TestsController : Controller
    {
        private readonly IRepository<Test> _testRepository;

        public TestsController(IRepository<Test> testRepository)
        {
            _testRepository = testRepository;
        }

        //[✔️] Сделать View
        //[✔️] Выводить список сущесвтвующих тестов
        // - разрешить не авторизованных пользователей
        [HttpGet]
        [HttpGet("index")]
        public async Task<IActionResult> Tests(int page)
        {
            const int pageSize = 10;
            var tests = await _testRepository.GetAllAsync();


            var testCards = tests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TestCardModel
                {
                    Id = t.Id,
                    Title = t.Title ?? "",
                    Description = t.Description ?? ""
                })
                .ToList();

            var totalCount = tests.Count();
            var paginated = new PaginatedTestCards(testCards, totalCount, page, pageSize);

            return View(paginated);
        }

        //[ ] Запись ответов в базу данных 
        // - разрешить не авторизованных пользователей
        [HttpPost("{guid}")]
        public IActionResult Test(Guid guid, TestViewModel model)
        {
            var answers = model.Answers;

            return Ok(answers);
        }

    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

    public class PaginatedTestCards : List<TestCardModel>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedTestCards(List<TestCardModel> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public bool IsFirstPage => PageIndex == 1;
        public bool IsLastPage => PageIndex == TotalPages;

        public static async Task<PaginatedTestCards> CreateAsync(
            IAsyncEnumerable<TestCardModel> source, int pageIndex, int pageSize)
        {
            var items = await source.Take(pageSize * pageIndex).Skip((pageIndex - 1) * pageSize).ToListAsync();
            var count = source.CountAsync().Result;
            return new PaginatedTestCards(items, count, pageIndex, pageSize);
        }
    }


}
