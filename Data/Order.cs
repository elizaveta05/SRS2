using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SRS2.Data
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } // Связь с пользователями
        public List<OrderProduct> OrderProducts { get; set; } // Промежуточная таблица
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; } // Общая сумма заказа
    }

    public class OrderProduct
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; } // Количество продукта в заказе
    }
}
