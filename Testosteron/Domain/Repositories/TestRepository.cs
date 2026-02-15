using Microsoft.EntityFrameworkCore;
using System;
using Testosteron.Data;
using Testosteron.Domain.Enities;
using Testosteron.Domain.Repositories.Base;

namespace Testosteron.Domain.Repositories
{
    public class TestRepository : BaseRepository<Test>
    {
        public TestRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
