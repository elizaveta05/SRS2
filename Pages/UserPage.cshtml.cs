using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SRS2.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SRS2.Pages
{
    public class UserPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UserPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // Идентификатор пользователя, переданный из Index

        [BindProperty]
        public int SelectedProductId { get; set; } // Идентификатор выбранного товара

        [BindProperty]
        public int SelectedQuantity { get; set; } // Количество выбранного товара

        public List<Product> Products { get; set; } // Список доступных товаров

        public List<Order> UserOrders { get; set; } // Список заказов текущего пользователя

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id <= 0)
            {
                return RedirectToPage("/Index");
            }

            // Получение списка доступных товаров
            Products = await _context.Products.ToListAsync();

            // Получение заказов пользователя по его ID
            UserOrders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.UserId == Id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostOrderAsync()
        {
            if (Id <= 0)
            {
                return RedirectToPage("/Index");
            }

            // Проверка, существует ли выбранный товар
            var product = await _context.Products.FindAsync(SelectedProductId);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Выбранный товар не найден.");
                return await OnGetAsync();
            }

            if (SelectedQuantity <= 0)
            {
                ModelState.AddModelError(string.Empty, "Количество должно быть больше 0.");
                return await OnGetAsync();
            }

            // Создание нового заказа
            var newOrder = new Order
            {
                UserId = Id,
                OrderDate = DateTime.Now,
                TotalAmount = product.Price * SelectedQuantity,
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct
                    {
                        ProductId = product.Id,
                        Quantity = SelectedQuantity
                    }
                }
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { Id }); 
        }
    }
}
