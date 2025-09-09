using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class VentaRequestDto
    {
        // Cliente y dirección
        public CustomerDto Customer { get; set; }

        // Shipping
        public string Shipping { get; set; }
        public ShippingOptionDto ShippingOption { get; set; }

        // Payment
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }

        // Items (el front SOLO manda product + quantity)
        public List<VentaItemRequestDto> Items { get; set; }

        // Total enviado por el front (se valida en el back)
        public decimal Total { get; set; }
    }

    public class CustomerDto
    {
        public int? ClientId { get; set; }  // null si es guest
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Dni { get; set; }

        // Dirección
        public string Street { get; set; }
        public string Number { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
    }

    public class VentaItemRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        // ❌ Ya no mandamos Subtotal ni UnitPrice → eso lo calcula el back
    }
}
