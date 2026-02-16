# Отчет по разработке системы онлайн-тестирования «Testosteron»

**Выполнил:** Команда разработки Testosteron  
**Дата:** Февраль 2025 г.  
**Версия:** 1.0

---

## Содержание

1. [Введение](#1-введение)
2. [Краткое описание структуры программы](#2-краткое-описание-структуры-программы)
3. [Основные части кода с объяснениями](#3-основные-части-кода-с-объяснениями)
4. [Скриншоты работы программы](#4-скриншоты-работы-программы)
5. [Выводы и предложения по улучшению](#5-выводы-и-предложения-по-улучшению)

---

# 1. Введение

## 1.1 Цели работы

**Целью** данной работы является разработка веб-приложения для проведения онлайн-тестирований с гибкой структурой, позволяющей создавать тесты с различными типами вопросов.

## 1.2 Задачи работы

Для достижения поставленной цели были решены следующие задачи:

1. **Проектирование архитектуры приложения**
   - Разработка трехуровневой архитектуры (Presentation → Business Logic → Data Access)
   - Проектирование структуры базы данных
   - Определение функциональных требований

2. **Реализация системы аутентификации и авторизации**
   - Интеграция ASP.NET Core Identity
   - Реализация ролевой модели доступа (admin, user)
   - Защита административной области

3. **Разработка функционала управления тестами**
   - CRUD-операции для тестов
   - Гибкая структура полей (text, radio, check)
   - Редактирование тестов в реальном времени

4. **Разработка интерфейса прохождения тестов**
   - Отображение тестов по уникальным ссылкам (GUID)
   - Поддержка различных типов полей
   - Асинхронная отправка ответов (AJAX)

5. **Тестирование и документирование**
   - Проверка работоспособности всех функций
   - Создание технической документации

## 1.3 Описание предметной области

Система онлайн-тестирования предназначена для:

- **Образовательных учреждений** – проведение экзаменов и контрольных работ
- **HR-отделов** – тестирование кандидатов при приеме на работу
- **Компаний** – оценка знаний сотрудников, аттестации
- **Маркетинговых исследований** – сбор обратной связи от клиентов

**Ключевые особенности:**

- Возможность создания тестов без программирования
- Поддержка трех типов вопросов:
  - Текстовые ответы (свободная форма)
  - Выбор одного варианта (radio)
  - Выбор нескольких вариантов (checkboxes)
- Хранение структуры теста в формате JSONB
- Прохождение тестов без регистрации (для гостей)
- Сохранение результатов для зарегистрированных пользователей

---

# 2. Краткое описание структуры программы

## 2.1 Общая архитектура

Система построена на базе **ASP.NET Core MVC** с использованием паттерна **Repository** для доступа к данным.

**Архитектура системы:**

1. **Клиент (Браузер):**
   - HTML/CSS/JS

2. **Presentation Layer (MVC):**
   - Controllers (Контроллеры)
   - Views (Представления)
   - Models (Модели)

3. **Business Logic Layer (Services):**
   - TestManager (логика создания тестов, обработки ответов, валидация данных)

4. **Data Access Layer (Repositories):**
   - IRepository<T> (абстракция доступа к данным, CRUD-операции)

5. **База данных (PostgreSQL):**
   - Users (Identity)
   - Tests (JSONB)
   - Answers (JSONB)

## 2.2 Структура проекта

- **Testosteron.slnx** - Файл решения
- **README.md** - Описание проекта

- **Testosteron/** - Основной проект
  - **Controllers/** - MVC Контроллеры
    - AccountController.cs (Аутентификация)
    - HomeController.cs (Главная страница)
    - TestController.cs (Прохождение тестов)
    - TestsController.cs (Список тестов)

  - **Areas/Admin/** - Административная часть
    - **Controllers/**
      - HomeController.cs (Панель администратора)
      - TestController.cs (Управление тестами)
    - **Models/**
      - EditTestViewModel.cs (Модель редактирования)
    - **Views/**
      - Home/ (Представления админки)
      - Test/ (Управление тестами)
    - **Components/**
      - TestFieldViewComponent.cs

  - **Domain/** - Доменная логика
    - **Entities/** - Сущности БД
      - Test.cs (Модель теста)
      - Answers.cs (Модель ответов)
      - ContentBase.cs (Базовые классы)
    - **Repositories/** - Репозитории
      - **Base/**
        - IRepositoy.cs (Интерфейс репозитория)
        - RepositoryBase.cs (Базовый репозиторий)
      - TestRepository.cs
      - AnswersRepository.cs
    - ApplicationDbContext.cs (Контекст EF Core)
    - ApplicationUser.cs (Пользователь)

  - **Services/** - Бизнес-логика
    - TestManager.cs (Менеджер тестов)
    - Result.cs (Паттерн Result<T>)
    - ResultJsonConverter.cs (JSON-сериализация)
    - AdminAreaAuthorization.cs (Авторизация)

  - **Models/** - View Models
    - TestViewModel.cs (Модель представления теста)
    - TestCardModel.cs (Модель карточки теста)
    - LoginModel.cs (Модель входа)
    - FieldAnswer.cs (Ответ на поле)
    - SubmitTestRequest.cs (Запрос отправки)

  - **Views/** - Представления
    - Shared/ (Общие представления)
    - Home/ (Главная страница)
    - Test/ (Прохождение теста)
    - Tests/ (Список тестов)

  - **wwwroot/** - Статические файлы
    - css/ (Стили)
    - js/ (Скрипты)
    - lib/ (Библиотеки: Bootstrap, jQuery)

  - Program.cs (Точка входа)
  - Testosteron.csproj (Файл проекта)

## 2.3 Технологический стек

| Компонент | Технология | Назначение |
|-----------|------------|------------|
| Язык программирования | C# (.NET 10) | Основной язык разработки |
| Веб-фреймворк | ASP.NET Core MVC | Веб-инфраструктура |
| ORM | Entity Framework Core 10 | Доступ к данным |
| База данных | PostgreSQL / SQLite | Хранение данных |
| Аутентификация | ASP.NET Core Identity | Управление пользователями |
| Фронтенд | Razor Views + Bootstrap 5 | Пользовательский интерфейс |
| JavaScript | jQuery 3.7 | Клиентские сценарии |
| CSS-фреймворк | Bootstrap 5 | Стилизация |

---

# 3. Основные части кода с объяснениями

## 3.1 Модели данных (Entities)

### 3.1.1 Модель Test (Тест)

```csharp
namespace Testosteron.Domain.Enities;

public class Test
{
    [Required]
    public Guid Id { get; set; }                    // Уникальный идентификатор

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty; // Название теста

    [Required, MaxLength(600)]
    public string Description { get; set; } = string.Empty; // Описание теста

    [Required, Column(TypeName = "jsonb")]
    public List<TestField> TestFields { get; set; } = new(); // Структура полей
}

public class TestField
{
    [Required]
    [Length(minimumLength: 0, maximumLength: 100)]
    public string Title { get; set; } = string.Empty;   // Вопрос

    [Required]
    public string TestFieldType { get; set; } = string.Empty; // text, radio, check

    [Required]
    public bool Required { get; set; }                   // Обязательное поле

    [Required]
    [Length(minimumLength: 0, maximumLength: 400)]
    public string Description { get; set; } = string.Empty; // Описание/подсказка

    [Required]
    public string[] Options { get; set; } = Array.Empty<string>(); // Варианты ответов
}
```

**Пояснение:**
- Модель `Test` представляет тест с уникальным `Id` (GUID)
- Поле `TestFields` хранится в формате **JSONB** для гибкости структуры
- `TestField` определяет вопрос с типом (`text`, `radio`, `check`) и опциями

### 3.1.2 Модель Answers (Ответы)

```csharp
public class Answers
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TestId { get; set; }        // Ссылка на тест

    public Guid? UserId { get; set; }       // Ссылка на пользователя (nullable)

    [Required, Column(TypeName = "jsonb")]
    public List<FieldAnswer> Content { get; set; } = new(); // Ответы пользователя
}
```

**Пояснение:**
- Хранит ответы пользователя на конкретный тест
- Поле `UserId` nullable – позволяет сохранять ответы гостей
- `Content` содержит структурированные ответы в формате JSONB

## 3.2 Репозитории (Data Access Layer)

### 3.2.1 Базовый интерфейс IRepository<T>

```csharp
namespace Testosteron.Domain.Repositories.Base;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
}
```

**Пояснение:**
- Обобщенный интерфейс для всех репозиториев
- Определяет базовые CRUD-операции
- `FindAsync` позволяет искать по предикату

### 3.2.2 Реализация TestRepository

```csharp
namespace Testosteron.Domain.Repositories;

public class TestRepository : IRepository<Test>
{
    private readonly ApplicationDbContext _context;

    public TestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Test> AddAsync(Test entity)
    {
        await _context.Tests.AddAsync(entity);
        return entity;
    }

    public async Task<Test?> GetByIdAsync(Guid id)
    {
        return await _context.Tests.FindAsync(id);
    }

    public async Task<IEnumerable<Test>> GetAllAsync()
    {
        return await _context.Tests.ToListAsync();
    }

    public async Task DeleteAsync(Test entity)
    {
        _context.Tests.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Test>> FindAsync(Func<Test, bool> predicate)
    {
        return await Task.FromResult(_context.Tests.Where(predicate));
    }
}
```

**Пояснение:**
- Реализует `IRepository<Test>` для работы с тестами
- Использует `ApplicationDbContext` для доступа к данным
- Все методы асинхронные для оптимизации производительности

## 3.3 Бизнес-логика (Services)

### 3.3.1 Паттерн Result<T>

```csharp
namespace Testosteron.Services;

public struct Result : IResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string[]? Errors { get; set; }

    public Result ErrorsFromException(Exception exception)
    {
        var errorMessages = new List<string>();
        if (exception.Message is not null)
            errorMessages.Add(exception.Message);
        if (exception.InnerException?.Message is { } innerMessage)
            errorMessages.Add(innerMessage);

        Errors = Errors == null 
            ? errorMessages.ToArray() 
            : Errors.Concat(errorMessages).ToArray();

        return this;
    }

    public static Result CreateSuccess(string message)
    {
        return new Result(true, null, message);
    }

    public static Result CreateFailure(string message = "", string[]? errors = null)
    {
        return new Result(false, errors ?? Array.Empty<string>(), message);
    }
}

public struct Result<T> : IResult<T>
{
    public bool Success { get; set; }
    public T Value { get; set; }
    public string Message { get; set; }
    public string[]? Errors { get; set; }

    public static Result<T> CreateSuccess(T value, string message = "")
    {
        return new Result<T>(value, true, null, message);
    }

    public static Result<T> CreateFailure(T value, string message = "", string[]? errors = null)
    {
        return new Result<T>(value, false, errors ?? Array.Empty<string>(), message);
    }
}
```

**Пояснение:**
- Паттерн `Result<T>` для унифицированной обработки ошибок
- Заменяет традиционные исключения и null-возвраты
- Содержит: `Success` (успех), `Value` (значение), `Message` (сообщение), `Errors` (ошибки)
- Методы `CreateSuccess` и `CreateFailure` для создания результатов

### 3.3.2 TestManager (основной сервис)

```csharp
namespace Testosteron.Services;

public class TestManager
{
    private readonly IRepository<Test> _testRepository;
    private readonly IRepository<Answers> _answersRepository;

    public TestManager(IRepository<Test> testRepository, IRepository<Answers> answersRepository)
    {
        _testRepository = testRepository;
        _answersRepository = answersRepository;
    }

    public async Task<Result<Test?>> GetTestByIdAsync(Guid id)
    {
        try
        {
            var test = await _testRepository.GetByIdAsync(id);

            if (test == null)
                return Result<Test?>.CreateFailure(default, $"Test not found");

            return Result<Test?>.CreateSuccess(test, "Test retrieved");
        }
        catch (Exception ex)
        {
            return Result<Test?>.CreateFailure(default, "Server error")
                .ErrorsFromException(ex);
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
            return Result<Answers?>.CreateFailure(default, "Server error")
                .ErrorsFromException(ex);
        }
    }

    public async Task<Result<Test?>> AddNewTestAsync(CreateTestDTO dto)
    {
        try
        {
            var result = await _testRepository.AddAsync(dto);

            if (result == null)
                return Result<Test?>.CreateFailure(default, "Test not found");

            await _testRepository.SaveChangesAsync();
            return Result<Test?>.CreateSuccess(result, "Test was created");
        }
        catch (Exception ex)
        {
            return Result<Test?>.CreateFailure(default, "Server error")
                .ErrorsFromException(ex);
        }
    }
}
```

**Пояснение:**
- `TestManager` – центральный сервис бизнес-логики
- Использует паттерн `Result<T>` для обработки операций
- Методы:
  - `GetTestByIdAsync` – получить тест по ID
  - `AddAnswersToTest` – сохранить ответы пользователя
  - `AddNewTestAsync` – создать новый тест
- Все методы обернуты в try-catch для обработки исключений

## 3.4 Контроллеры (Presentation Layer)

### 3.4.1 TestController (прохождение тестов)

```csharp
namespace Testosteron.Controllers;

[Route("Test")]
public class TestController : Controller
{
    private readonly TestManager _testManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public TestController(TestManager testManager, UserManager<ApplicationUser> userManager)
    {
        _testManager = testManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Test(Guid guid)
    {
        var result = await _testManager.GetTestByIdAsync(guid);

        if (!result.Success)
            return this.NotFound($"Страница Test/{guid} не найдена");

        TestViewModel model = new TestViewModel(result.Value!);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Test([FromForm] TestViewModel viewModel)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        Guid? userId = currentUser == null ? null : currentUser.Id;

        if (userId.HasValue)
        {
            var answers = await _testManager.GetUserAnswersForTest(userId.Value, viewModel.TestId);
            if (answers.Success)
                return Json(new { success = false, message = "Вы уже дали ответ на этот тест" });
        }

        var result = await _testManager.AddAnswersToTest(new() { 
            TestId = viewModel.TestId, 
            UserId = userId 
        });

        return Json(new { success = true, message = "Ответ сохранен" });
    }
}
```

**Пояснение:**
- Маршрут `/Test` для прохождения тестов
- `GET /Test?guid=XXX` – отображает страницу теста
- `POST /Test` – обрабатывает отправку ответов
- Проверяет, не проходил ли пользователь тест ранее
- Возвращает JSON-ответ с результатом операции

### 3.4.2 Admin Area TestController

```csharp
namespace Testosteron.Areas.Admin.Controllers;

[Area("Admin")]
public class TestController : Controller
{
    private readonly IRepository<Test> _testRepository;
    private readonly TestManager _testManager;

    [HttpGet]
    public async Task<IActionResult> Edit(Guid guid)
    {
        var test = await _testManager.GetTestByIdAsync(guid);

        if (!test.Success)
            return NotFound(new { success = false, message = test.Message });

        EditTestViewModel vm = new EditTestViewModel(test.Value!);
        return View("EditTest", vm);
    }

    [HttpPatch]
    public async Task<IActionResult> Edit(Guid guid, [FromForm] EditTestViewModel model)
    {
        UpdateTestDTO update = new()
        {
            Id = guid,
            Description = model.Description,
            TestFields = model.Fields.Select(item => item.ToTestField()).ToList(),
            Title = model.TestTitle
        };

        var result = await _testManager.UpdateTestAsync(update);
        return Json(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid guid)
    {
        var result = await _testManager.DeleteTestAsync(new() { Id = guid });
        return Json(result);
    }
}
```

**Пояснение:**
- Расположен в области `Admin`
- `GET /Admin/Test?guid={guid}` – страница редактирования теста
- `PATCH /Admin/Test?guid={guid}` – обновление теста
- `DELETE /Admin/Test?guid={guid}` – удаление теста
- Все операции возвращают JSON для AJAX-запросов

## 3.5 Представления (Views)

### 3.5.1 Страница прохождения теста (Test.cshtml)

```html
@model TestViewModel

<form id="form" action="/Test" method="post" class="test container mt-4">
    @Html.AntiForgeryToken()
    <input type="hidden" asp-for="TestId" />

    <h1 class="mb-4">@Model.TestTitle</h1>
    
    @if (!string.IsNullOrEmpty(Model.TestDescription))
    {
        <div class="mb-4">
            <h6>@Model.TestDescription</h6>
        </div>
    }

    @for (int i = 0; i < Model.TestFields.Count(); i++)
    {
        var field = Model.TestFields.ElementAt(i);
        <div class="card mb-3">
            <div class="card-body">
                <h3 class="card-title mb-3">@field.Title</h3>
                <input type="hidden" name="answers[@i].fieldType" value="@field.TestFieldType" />

                @switch (field.TestFieldType?.ToLower())
                {
                    case "check":
                        @for (int j = 0; j < field.Options.Length; j++)
                        {
                            <div class="form-check mb-2">
                                <input name="answers[@i].checkboxValues[@j]" 
                                       class="form-check-input" type="checkbox" value="true" />
                                <input type="hidden" name="answers[@i].checkboxValues[@j]" value="false" />
                                <label class="form-check-label">@field.Options[j]</label>
                            </div>
                        }
                        break;

                    case "radio":
                        @for (int j = 0; j < field.Options.Length; j++)
                        {
                            <div class="form-check mb-2">
                                <input name="answers[@i].radioIndex"
                                       class="form-check-input" type="radio" value="@j" />
                                <label class="form-check-label">@field.Options[j]</label>
                            </div>
                        }
                        break;

                    default:
                        @for (int j = 0; j < field.Options.Length; j++)
                        {
                            <div class="mb-3">
                                <label class="form-label">@field.Options[j]</label>
                                <textarea name="answers[@i].textValue" class="form-control" rows="3"></textarea>
                            </div>
                        }
                        break;
                }
            </div>
        </div>
    }

    <div class="mt-4">
        <div id="alertPlaceholder"></div>
        <button type="submit" class="btn btn-primary btn-lg">Отправить</button>
    </div>
</form>
```

**Пояснение:**
- Генерирует форму на основе структуры `TestFields`
- Поддерживает три типа полей:
  - `check` – чекбоксы для множественного выбора
  - `radio` – радио-кнопки для одиночного выбора
  - `text` – текстовые поля (textarea)
- Использует Bootstrap 5 для стилизации

### 3.5.2 JavaScript для отправки формы

```javascript
$("#form").submit(function(e) {
    e.preventDefault();
    const formData = new FormData(this);
    fetch("test", {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(text || "HTTP " + response.status);
            });
        }
        return response.text().then(text => {
            try { return JSON.parse(text); }
            catch { return text; }
        });
    })
    .then(result => {
        if(result.success) {
            showAlert('success', result.message);
        } else {
            showAlert('danger', result.message);
        }
    })
    .catch(error => {
        showAlert('danger', error.message);
    });
});
```

**Пояснение:**
- AJAX-отправка формы без перезагрузки страницы
- Обрабатывает JSON-ответы от сервера
- Отображает уведомления о результате (успех/ошибка)

## 3.6 Конфигурация приложения (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Настройка базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var sqliteConnectionString = builder.Configuration.GetConnectionString("DefaultConnection-development");
        options.UseNpgsql(sqliteConnectionString);  // Для разработки
    }
    else
    {
        var postgresConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(postgresConnectionString); // Для production
    }
});

// Регистрация сервисов
builder.Services.AddScoped<IRepository<Test>, TestRepository>();
builder.Services.AddScoped<IRepository<Answers>, AnswersRepository>();
builder.Services.AddScoped<TestManager>();

// Настройка Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Настройка авторизации для Admin Area
builder.Services.AddControllersWithViews((x)=>
{
    x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
});

// Автоматическое создание админа
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    string adminRole = "admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));

    var adminUser = await userManager.FindByEmailAsync("my@email.com");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser { 
            UserName = "admin", 
            Email = "my@email.com", 
            EmailConfirmed = true 
        };
        await userManager.CreateAsync(adminUser, "superpassword");
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}

// Настройка маршрутов
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

**Пояснение:**
- Настройка подключения к БД (PostgreSQL)
- Регистрация репозиториев и сервисов в DI-контейнере
- Конфигурация ASP.NET Identity с минимальными требованиями к паролю
- Настройка маршрутизации для Admin Area
- Автоматическое создание администратора при первом запуске

---

# 4. Скриншоты работы программы

## 4.1 Главная страница

**Описание:** Главная страница приложения с навигацией и списком доступных тестов.

место для скриншота

## 4.2 Страница прохождения теста

**Описание:** Страница с вопросами теста, отображаемая по уникальной ссылке (GUID).

место для скриншота

## 4.3 Административная панель - список тестов

**Описание:** Панель администратора со списком созданных тестов.

место для скриншота

## 4.4 Административная панель - редактирование теста

**Описание:** Форма редактирования теста с возможностью добавления/удаления полей.

место для скриншота

## 4.5 Страница входа

**Описание:** Страница аутентификации пользователей.

место для скриншота

## 4.6 Сообщение об успешной отправке

**Описание:** Уведомление после успешного прохождения теста.

место для скриншота

# 5. Выводы и предложения по улучшению

## 5.1 Выводы

В ходе выполнения работы была разработана **система онлайн-тестирования «Testosteron»**, включающая следующие возможности:

### 5.1.1 Реализованный функционал

**Аутентификация и авторизация**
- Регистрация и вход пользователей
- Ролевая модель доступа (admin, user)
- Защита административной области

**Управление тестами (CRUD)**
- Создание тестов с гибкой структурой
- Редактирование тестов
- Удаление тестов
- Предпросмотр тестов

**Прохождение тестов**
- Уникальные ссылки для каждого теста (GUID)
- Поддержка трех типов вопросов (text, radio, check)
- Асинхронная отправка ответов (AJAX)
- Проверка на повторное прохождение

**Техническая реализация**
- Трехуровневая архитектура (MVC + Services + Repositories)
- Entity Framework Core для доступа к данным
- PostgreSQL с поддержкой JSONB
- ASP.NET Core Identity
- Паттерн Result<T> для обработки ошибок

### 5.1.2 Преимущества разработанной системы

| Преимущество | Описание |
|-------------|----------|
| **Гибкость** | JSONB-структура позволяет хранить любую структуру тестов |
| **Масштабируемость** | Трехуровневая архитектура легко расширяется |
| **Безопасность** | ASP.NET Identity + ролевая модель |
| **Кроссплатформенность** | Работает на Windows, Linux, macOS |
| **Простота развертывания** | Docker-контейнер, стандартный стек технологий |

### 5.1.3 Технологические решения

- **ASP.NET Core MVC** – проверенный веб-фреймворк с широкими возможностями
- **Entity Framework Core** – современный ORM с миграциями
- **PostgreSQL** – надежная СУБД с поддержкой JSONB
- **Bootstrap 5 + jQuery** – быстрая разработка интерфейса

## 5.2 Предложения по улучшению

### 5.2.1 Краткосрочные улучшения (1-2 месяца)

**1. Добавление типов полей**
| Тип поля | Описание | Приоритет |
|----------|----------|-----------|
| `dropdown` | Выпадающий список | Средний |
| `file` | Загрузка файлов | Низкий |
| `scale` | Шкала оценки (1-10) | Средний |
| `date` | Выбор даты | Низкий |
| `matrix` | Матрица вопросов | Низкий |

**2. Улучшение UI/UX**

- Валидация полей в реальном времени
- Drag-and-drop для сортировки вопросов
- Превью изображений при загрузке
- Прогресс-бар при прохождении
- Таймер на прохождение теста
- Адаптивная версия для мобильных

**3. Оптимизация производительности**
- Кэширование часто запрашиваемых тестов
- Пагинация для списка тестов
- Ленивая загрузка изображений
- Минификация статических файлов

### 5.2.2 Среднесрочные улучшения (3-6 месяцев)

**1. Система результатов и аналитики**

- **Статистика прохождений:**
  - Количество прохождений
  - Средний балл
  - Время прохождения

- **Графики распределения ответов:**
  - Процент правильных ответов
  - Сложные вопросы (низкий % правильных ответов)
  - Популярные неправильные ответы

- **Аналитика по пользователям:**
  - История прохождений
  - Прогресс обучения
  - Сравнение с другими

**2. Импорт/экспорт тестов**
- Экспорт тестов в стандартные форматы (QTI, SCORM)
- Импорт тестов из Excel/CSV
- Шаблоны тестов

**3. Командная работа**
- Множественные роли (author, reviewer, publisher)
- Версионирование тестов
- Комментарии и обсуждения
- Уведомления о изменениях

### 5.2.3 Долгосрочные улучшения (6-12 месяцев)

**1. Интеграции**

| Интеграция | Описание | Сложность |
|------------|----------|-----------|
| LMS (Moodle, Canvas) | Интеграция с системами управления обучением | Высокая |
| SSO (Single Sign-On) | Единая точка входа (OAuth 2.0, SAML) | Средняя |
| Webhooks | Уведомления о событиях | Низкая |
| API (REST/GraphQL) | Программный доступ к системе | Средняя |

**2. Расширенные типы вопросов**

- **Эссе с автоматической проверкой:**
  - Проверка ключевых слов
  - Анализ тошноты текста
  - Оценка длины ответа

- **Математические задачи:**
  - Генерация вариантов
  - Проверка формул
  - Пошаговые решения

- **Сопоставление изображений:**
  - Drag-and-drop сопоставление
  - Горячие точки на изображении

- **Кластеризация ответов:**
  - Автоматическая группировка
  - Анализ открытых ответов

**3. Масштабирование**

- **Стратегия горизонтального масштабирования:**
  - Load Balancer (Nginx)
  - Несколько App Servers (Stateless)
  - Redis Cache (Session, Cache)
  - PostgreSQL (Primary + Replicas)

### 5.2.4 Рекомендации по приоритетам

- **Высокий приоритет (сделать в первую очередь):**
  1. Валидация полей в реальном времени
  2. Пагинация и поиск по тестам
  3. API для интеграций

- **Средний приоритет (следующий этап):**
  1. Расширенные типы полей (dropdown, scale)
  2. Drag-and-drop сортировка
  3. Импорт/экспорт тестов

- **Низкий приоритет (когда будет время):**
  1. SSO интеграция
  2. Математические задачи
  3. Сложная аналитика

## 5.3 Технические рекомендации

### 5.3.1 Мониторинг и observability

- **Метрики (Prometheus + Grafana):**
  - Время ответа API
  - Количество запросов в секунду
  - Использование памяти/CPU
  - Количество активных пользователей

- **Логирование (ELK Stack / Seq):**
  - Структурированные логи (JSON)
  - Уровни логирования (Info, Warn, Error)
  - Трейсинг запросов (correlation ID)

- **Алертинг:**
  - Ошибки 5xx
  - Высокое время ответа (>2s)
  - Недоступность сервисов

### 5.3.2 CI/CD Pipeline

1. **Code Commit (Git)**
2. **Build & Test (GitHub Actions / Azure DevOps):**
   - Компиляция
   - Модульные тесты
   - Статический анализ (SonarQube)
   - Проверка безопасности (Snyk)

3. **Staging Deployment:**
   - Docker сборка
   - Интеграционные тесты

4. **Production Deployment (Blue/Green или Canary):**
   - Миграции БД
   - Health checks
   - Rollback план

---

## Заключение

В рамках данной работы была разработана функциональная система онлайн-тестирования «Testosteron», соответствующая поставленным требованиям. Система построена на современном технологическом стеке и имеет потенциал для дальнейшего развития.

**Ключевые достижения:**

1. Реализована полнофункциональная система управления тестами
2. Обеспечена безопасность на уровне аутентификации и авторизации
3. Достигнута гибкость структуры данных через JSONB
4. Обеспечен дружественный пользовательский интерфейс
5. Создана документация (ТЗ, руководства, отчеты)

**Пути развития:**

Система готова к расширению функционала в соответствии с предложенными улучшениями. Архитектура позволяет добавлять новые возможности без существенной переработки существующего кода.

---

*Отчет подготовлен: Февраль 2025 г.*  
*Версия: 1.0*
