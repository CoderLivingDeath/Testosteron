using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testosteron.Data;
using Testosteron.Domain;
using Testosteron.Domain.Enities;
using Testosteron.Domain.Repositories;
using Testosteron.Domain.Repositories.Base;
using Testosteron.Services;

namespace Testosteron
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    var sqliteConnectionString = builder.Configuration.GetConnectionString("DefaultConnection-development");
                    options.UseNpgsql(sqliteConnectionString);
                }
                else
                {
                    var postgresConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                    options.UseNpgsql(postgresConnectionString);
                }
            });
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IRepository<Test>, TestRepository>();
            builder.Services.AddScoped<IRepository<Answers>, AnswersRepository>();
            builder.Services.AddScoped<TestManager>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // Отключаем подтверждение email

                // Минимальные требования к паролю
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;

                // Отключаем блокировку
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "TestosteronInc";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });

            builder.Services.AddControllersWithViews((x)=>
            {
                x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
            });

            builder.Services.AddAuthorization((x) =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                Task.Run(async () =>
                {
                    string adminRole = "admin";
                    if (!await roleManager.RoleExistsAsync(adminRole))
                        await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));

                    var adminUser = await userManager.FindByEmailAsync("my@email.com");
                    if (adminUser == null)
                    {
                        // 1. Создаём пользователя
                        adminUser = new ApplicationUser { UserName = "admin", Email = "my@email.com", EmailConfirmed = true };
                        var createResult = await userManager.CreateAsync(adminUser, "superpassword");

                        if (!createResult.Succeeded)
                        {
                            Console.WriteLine("Ошибка создания пользователя");
                            return;
                        }

                        // 2. Ждём сохранения в БД и проверяем существование
                        await Task.Delay(100); // Небольшая пауза для PostgreSQL

                        // 3. Добавляем роль только после успешного сохранения
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }
                }).GetAwaiter().GetResult();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();


            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.MapControllers();

            app.Run();

        }
    }
}
