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

            // ������������� ����������� �������
            var passwordHasher = new PasswordHasher<User>();

            try
            {
                // ����� ������������ �� ������
                var user = _context.Users.FirstOrDefault(u => u.Login == Login);

                if (user == null)
                {
                    // ���� ������������ �� ������
                    ModelState.AddModelError(string.Empty, "������������ �� ����������. �����������������.");
                    return Page();
                }

                // �������� ���� ������
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // �������� ����
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
                        ModelState.AddModelError(string.Empty, "� ��� ������������ ����.");
                    }
                }
                else
                {
                    // ���� ������ ��������
                    ModelState.AddModelError(string.Empty, "������������ ����� ��� ������.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� ��������� ������ ��� ������.");
                ModelState.AddModelError(string.Empty, "��������� ������. ���������� �����.");
            }

            return Page();
        }


    }
}
