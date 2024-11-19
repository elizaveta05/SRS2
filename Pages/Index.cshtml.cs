using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRS2.Data;

namespace SRS2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string Login { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            // Инициализация хэшировщика паролей
            var passwordHasher = new PasswordHasher<User>();

            try
            {
                // Поиск пользователя по логину
                var user = _context.Users.FirstOrDefault(u => u.Login == Login);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователя не существует.Зарегистрируйтесь.");
                }

                // Проверка хэша пароля
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Перенаправление на домашнюю страницу
                    return RedirectToPage("/HomePage");
                }
                else
                {
                    // Если пароль неверный
                    ModelState.AddModelError(string.Empty, "Неправильный логин или пароль.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке логина или пароля.");
                ModelState.AddModelError(string.Empty, "Произошла ошибка. Попробуйте позже.");
            }

            return Page();
        }
    }
}
