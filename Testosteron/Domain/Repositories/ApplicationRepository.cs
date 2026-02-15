using Microsoft.EntityFrameworkCore;
using Testosteron.Data;
using Testosteron.Domain.Repositories.Base;

namespace Testosteron.Domain.Repositories
{
    public class ApplicationRepository<T> : BaseRepository<T> where T : class
    {
        public ApplicationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
