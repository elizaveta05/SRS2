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

        [BindProperty]
        public string Role { get; set; }

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
                    // Если пользователь не найден
                    ModelState.AddModelError(string.Empty, "Пользователя не существует. Зарегистрируйтесь.");
                    return Page();
                }

                // Проверка хэша пароля
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Проверка роли
                    if (user.Role == "Moderator")
                    {
                        return RedirectToPage("/ModeratorPage", new { id = user.Id });
                    }
                    else if (user.Role == "User")
                    {
                        return RedirectToPage("/UserPage", new { id = user.Id });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "У вас недостаточно прав.");
                    }
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
