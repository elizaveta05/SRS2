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
        public int Id { get; set; } // ������������� ������������, ���������� �� Index

        [BindProperty]
        public int SelectedProductId { get; set; } // ������������� ���������� ������

        [BindProperty]
        public int SelectedQuantity { get; set; } // ���������� ���������� ������

        public List<Product> Products { get; set; } // ������ ��������� �������

        public List<Order> UserOrders { get; set; } // ������ ������� �������� ������������

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id <= 0)
            {
                return RedirectToPage("/Index");
            }

            // ��������� ������ ��������� �������
            Products = await _context.Products.ToListAsync();

            // ��������� ������� ������������ �� ��� ID
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

            // ��������, ���������� �� ��������� �����
            var product = await _context.Products.FindAsync(SelectedProductId);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "��������� ����� �� ������.");
                return await OnGetAsync();
            }

            if (SelectedQuantity <= 0)
            {
                ModelState.AddModelError(string.Empty, "���������� ������ ���� ������ 0.");
                return await OnGetAsync();
            }

            // �������� ������ ������
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
