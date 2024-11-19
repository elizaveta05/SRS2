using Microsoft.EntityFrameworkCore;
using SRS2.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем ApplicationDbContext с использованием строки подключения из appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем службы, необходимые для работы с Razor Pages.
builder.Services.AddRazorPages();

var app = builder.Build();

// Конфигурация конвейера обработки HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // Значение HSTS по умолчанию — 30 дней. Вы можете изменить это значение для сценариев в производстве.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
