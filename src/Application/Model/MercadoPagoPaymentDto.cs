using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class MercadoPagoPaymentDto
    {
        public string Id { get; set; }
        public string Status { get; set; } // approved, pending, rejected
        public string StatusDetail { get; set; }
        public string ExternalReference { get; set; } // coincide con tu pedido
        public decimal TransactionAmount { get; set; }
        public PayerDto Payer { get; set; }

        public class PayerDto
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
    }
}
