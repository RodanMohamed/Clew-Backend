using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class OrderReadDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public IEnumerable<OrderItemReadDto> Items { get; set; } = new List<OrderItemReadDto>();
    }
}
