using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using Testosteron.Domain;
using Testosteron.Domain.Enities;

namespace Testosteron.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<Answers> TestsAnswers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public ApplicationDbContext()
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var testFieldsConverter = new ValueConverter<List<TestField>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<TestField>>(v, (JsonSerializerOptions)null!) ?? new List<TestField>()
            );

            var fieldAnswerConverter = new ValueConverter<AnswersContainer, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<AnswersContainer>(v, (JsonSerializerOptions)null!) ?? new AnswersContainer()
            );

            modelBuilder.Entity<Test>(entity =>
            {
                entity.Property(e => e.TestFields)
                    .HasConversion(testFieldsConverter)
                    .HasColumnType("jsonb");
            });

            modelBuilder.Entity<Answers>(entity =>
            {
                entity.Property(e => e.Content)
                    .HasConversion(fieldAnswerConverter)
                    .HasColumnType("jsonb");
            });
        }
    }
}
