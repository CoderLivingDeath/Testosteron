using Microsoft.EntityFrameworkCore;
using Testosteron.Data;
using Testosteron.Domain.Enities;

namespace Testosteron.Domain.Repositories
{
    public class AnswersRepository : ApplicationRepository<Answers>
    {
        public AnswersRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
