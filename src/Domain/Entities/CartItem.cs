namespace Domain.Entities
{

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Cantidad { get; set; }  // <--- ESTA PROPIEDAD ES NECESARIA
        public decimal PrecioUnitario { get; set; } // O Price, o UnitPrice, lo que uses

        // Podrías agregar una propiedad calculada:
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
