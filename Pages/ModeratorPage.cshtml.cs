using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SRS2.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS2.Pages
{
    public class ModeratorPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ModeratorPageModel(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<User> Users { get; set; } // Список пользователей
        public List<Product> Products { get; set; } // Список продуктов
        public List<Order> Orders { get; set; } // Список заказов

        [BindProperty]
        public User EditableUser { get; set; } // Пользователь для редактирования

        [BindProperty]
        public Product EditableProduct { get; set; } // Продукт для редактирования

        [BindProperty]
        public Order EditableOrder { get; set; } // Заказ для редактирования

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
            Products = await _context.Products.ToListAsync();
            Orders = await _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (EditableUser != null && EditableUser.Id > 0)
            {
                var user = await _context.Users.FindAsync(EditableUser.Id);
                if (user != null)
                {
                    user.Login = EditableUser.Login;
                    user.Role = EditableUser.Role;

                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditProductAsync()
        {
            if (EditableProduct != null && EditableProduct.Id > 0)
            {
                var product = await _context.Products.FindAsync(EditableProduct.Id);
                if (product != null)
                {
                    product.Name = EditableProduct.Name;
                    product.Price = EditableProduct.Price;
                    if (EditableProduct.Stock >= 0)
                    {
                        product.Stock = EditableProduct.Stock;
                    }
                    else
                    {
                        ModelState.AddModelError("EditableProduct.Stock", "Количество не может быть отрицательным.");
                        return Page();
                    }

                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditOrderAsync()
        {
            if (EditableOrder != null && EditableOrder.Id > 0)
            {
                var order = await _context.Orders.FindAsync(EditableOrder.Id);
                if (order != null)
                {
                    order.TotalAmount = EditableOrder.TotalAmount;

                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
