using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;



namespace Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartService _cartService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;

        public VentaService(
            IVentaRepository ventaRepository,
            ICartService cartService,
            IProductRepository productRepository,
            IPaymentService paymentService,
            IShippingService shippingService
        )
        {
            _ventaRepository = ventaRepository;
            _cartService = cartService;
            _productRepository = productRepository;
            _paymentService = paymentService;
            _shippingService = shippingService;
        }

        // 🔹 Entidad → DTO
        private VentaResponseDto MapToDto(Venta v)
        {
            return new VentaResponseDto
            {
                OrderId = v.Id,
                ClientId = v.ClientId,
                CustomerEmail = v.Client?.Email ?? v.CustomerEmail ?? string.Empty,
                CustomerName = v.Client != null ? v.Client.Name : v.CustomerName,
                CustomerLastname = v.Client != null ? v.Client.LastName : v.CustomerLastname,
                Street = v.Street,
                Number = v.Number,
                Department = v.Department,
                Description = v.Description,
                City = v.City,
                Province = v.Province,
                PostalCode = v.PostalCode,
                ShippingMethod = v.ShippingMethod,
                ShippingCost = v.ShippingCost,
                PaymentMethod = v.PaymentMethod,
                Notes = v.Notes,
                Date = v.Date,
                Status = v.Status.ToString(),
                Total = v.Total,
                ExternalReference = v.ExternalReference ?? string.Empty,
                Items = v.DetalleVentas.Select(d => new VentaItemDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name ?? string.Empty,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public List<VentaResponseDto> GetAll()
        {
            var ventas = _ventaRepository.GetAll() ?? new List<Venta>();
            return ventas.Select(MapToDto).ToList();
        }

        public List<VentaResponseDto> GetAllByClient(int clientId)
        {
            var ventas = _ventaRepository.GetAllByClient(clientId) ?? new List<Venta>();
            return ventas.Select(MapToDto).ToList();
        }

        public VentaResponseDto? GetById(int id)
        {
            var venta = _ventaRepository.Get(id);
            return venta == null ? null : MapToDto(venta);
        }

        public int AddVenta(VentaDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("No se enviaron productos en la venta.");

            var shippingOptions = _shippingService.Calculate(dto.PostalCode);
            var selectedShipping = shippingOptions.FirstOrDefault(o => o.Name == dto.ShippingMethod);

            var venta = new Venta
            {
                ClientId = dto.ClientId > 0 ? dto.ClientId : null,
                CustomerEmail = dto.CustomerEmail,
                CustomerName = dto.CustomerName,
                CustomerLastname = dto.CustomerLastname,
                ExternalReference = dto.ExternalReference,
                Date = DateTime.Now,
                Status = VentaStatus.Pendiente,
                Street = dto.Street,
                Number = dto.Number,
                Department = dto.Department,
                Description = dto.Description,
                City = dto.City,
                Province = dto.Province,
                PostalCode = dto.PostalCode,
                PaymentMethod = dto.PaymentMethod,
                ShippingMethod = dto.ShippingMethod,
                DeliveryMethod = dto.ShippingMethod,
                ShippingCost = selectedShipping?.Cost ?? 0,
                Notes = dto.Notes,
                DetalleVentas = new List<DetalleVenta>()
            };

            decimal totalVenta = 0;

            foreach (var item in dto.Items)
            {
                var product = _productRepository.GetById(item.ProductId)
                    ?? throw new Exception($"Producto {item.ProductId} no encontrado");

                if (product.Stock < item.Quantity)
                    throw new Exception($"Stock insuficiente para el producto {product.Name}");

                var subtotal = item.Quantity * product.Price;

                venta.DetalleVentas.Add(new DetalleVenta
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });

                totalVenta += subtotal;

                // 🔹 Descontar stock
                product.Stock -= item.Quantity;
                _productRepository.Update(product);
            }

            venta.Total = totalVenta + venta.ShippingCost;

            var ventaGuardada = _ventaRepository.Add(venta);
            return ventaGuardada.Id;
        }

        public void UpdateVenta(Venta venta)
        {
            var existingVenta = _ventaRepository.Get(venta.Id)
                ?? throw new Exception($"Venta con ID {venta.Id} no encontrada.");

            // Restaurar stock de detalles antiguos
            foreach (var detalle in existingVenta.DetalleVentas)
            {
                var product = _productRepository.GetById(detalle.ProductId);
                if (product != null)
                {
                    product.Stock += detalle.Quantity;
                    _productRepository.Update(product);
                }
            }
            existingVenta.DetalleVentas.Clear();

            existingVenta.Total = venta.Total;
            existingVenta.Date = DateTime.Now;
            existingVenta.ClientId = venta.ClientId;
            existingVenta.CustomerEmail = venta.CustomerEmail;
            existingVenta.CustomerName = venta.CustomerName;
            existingVenta.CustomerLastname = venta.CustomerLastname;
            existingVenta.Street = venta.Street;
            existingVenta.Number = venta.Number;
            existingVenta.Department = venta.Department;
            existingVenta.Description = venta.Description;
            existingVenta.City = venta.City;
            existingVenta.Province = venta.Province;
            existingVenta.PostalCode = venta.PostalCode;
            existingVenta.PaymentMethod = venta.PaymentMethod;
            existingVenta.ShippingMethod = venta.ShippingMethod;
            existingVenta.Notes = venta.Notes;

            _ventaRepository.Update(existingVenta);
        }

        public VentaResponseDto UpdateVenta(int id, VentaDto dto)
        {
            var existingVenta = _ventaRepository.GetById(id)
                ?? throw new Exception($"Venta con ID {id} no encontrada.");

            // Restaurar stock de detalles antiguos
            foreach (var detalle in existingVenta.DetalleVentas)
            {
                var product = _productRepository.GetById(detalle.ProductId);
                if (product != null)
                {
                    product.Stock += detalle.Quantity;
                    _productRepository.Update(product);
                }
            }
            existingVenta.DetalleVentas.Clear();

            // 🔹 Si vienen items en el dto, los volvemos a cargar
            decimal totalVenta = 0;
            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var item in dto.Items)
                {
                    var product = _productRepository.GetById(item.ProductId)
                        ?? throw new Exception($"Producto {item.ProductId} no encontrado");

                    if (product.Stock < item.Quantity)
                        throw new Exception($"Stock insuficiente para el producto {product.Name}");

                    var subtotal = item.Quantity * product.Price;

                    existingVenta.DetalleVentas.Add(new DetalleVenta
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = subtotal
                    });

                    totalVenta += subtotal;

                    product.Stock -= item.Quantity;
                    _productRepository.Update(product);
                }
            }

            existingVenta.Total = totalVenta + dto.ShippingCost;
            existingVenta.Date = DateTime.Now;
            existingVenta.ClientId = dto.ClientId > 0 ? dto.ClientId : null;
            existingVenta.CustomerEmail = dto.CustomerEmail;
            existingVenta.CustomerName = dto.CustomerName;
            existingVenta.CustomerLastname = dto.CustomerLastname;
            existingVenta.Street = dto.Street;
            existingVenta.Number = dto.Number;
            existingVenta.Department = dto.Department;
            existingVenta.Description = dto.Description;
            existingVenta.City = dto.City;
            existingVenta.Province = dto.Province;
            existingVenta.PostalCode = dto.PostalCode;
            existingVenta.PaymentMethod = dto.PaymentMethod;
            existingVenta.ShippingMethod = dto.ShippingMethod;
            existingVenta.Notes = dto.Notes;

            _ventaRepository.Update(existingVenta);

            return MapToDto(existingVenta);
        }

        public void DeleteVenta(int id)
        {
            var venta = _ventaRepository.Get(id) ?? throw new Exception($"Venta con ID {id} no encontrada.");
            _ventaRepository.Delete(venta);
        }

        public void CancelVenta(int id)
        {
            var venta = _ventaRepository.Get(id) ?? throw new Exception("Venta no encontrada");

            if (venta.Status == VentaStatus.Cancelado)
                throw new Exception("La venta ya fue cancelada");

            foreach (var detalle in venta.DetalleVentas)
            {
                var product = _productRepository.GetById(detalle.ProductId);
                if (product != null)
                {
                    product.Stock += detalle.Quantity;
                    _productRepository.Update(product);
                }
            }

            venta.Status = VentaStatus.Cancelado;
            _ventaRepository.Update(venta);
        }

        public async Task<VentaResponseDto> CreateFromCart(int clientId, VentaDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var cart = _cartService.GetCartByClientId(clientId);
            if (cart == null || cart.Items == null || cart.Items.Count == 0)
                throw new Exception("Carrito vacío");

            var externalReference = Guid.NewGuid().ToString();

            var venta = new Venta
            {
                ClientId = clientId,
                Date = DateTime.Now,
                Status = VentaStatus.Pendiente,
                DetalleVentas = new List<DetalleVenta>(),
                CustomerEmail = dto.CustomerEmail,
                CustomerName = dto.CustomerName,
                CustomerLastname = dto.CustomerLastname,
                Street = dto.Street,
                Number = dto.Number,
                Department = dto.Department,
                Description = dto.Description,
                City = dto.City,
                Province = dto.Province,
                PostalCode = dto.PostalCode,
                PaymentMethod = dto.PaymentMethod,
                ShippingMethod = dto.ShippingMethod,
                ShippingCost = dto.ShippingCost,
                Notes = dto.Notes,
                ExternalReference = externalReference
            };

            decimal totalVenta = 0;

            // 🔹 Obtener todos los productos una sola vez
            var products = cart.Items
                .Select(i => _productRepository.GetById(i.ProductId))
                .ToDictionary(p => p.Id);

            foreach (var item in cart.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new Exception($"Producto {item.ProductId} no encontrado");

                var subtotal = item.Quantity * product.Price;

                venta.DetalleVentas.Add(new DetalleVenta
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });

                totalVenta += subtotal;
            }

            venta.Total = totalVenta + dto.ShippingCost;

            // 🔹 Guardar la venta en estado pendiente
            var ventaGuardada = _ventaRepository.Add(venta);

            // 🔹 Crear preferencia en Mercado Pago
            var checkoutRequest = new CheckoutRequestDto
            {
                Items = cart.Items.Select(i => new CheckoutItemDto
                {
                    Title = products[i.ProductId].Name,
                    Quantity = i.Quantity,
                    UnitPrice = products[i.ProductId].Price
                }).ToList(),
                PayerEmail = dto.CustomerEmail,
                ExternalReference = externalReference,
                NotificationUrl = "https://427eeb99434f.ngrok-free.app/api/Payment/webhook",

                BackUrls = new BackUrlsDto
                {
                    Success = "https://localhost:5173/checkout/success",
                    Failure = "https://localhost:5173/checkout/failure",
                    Pending = "https://localhost:5173/checkout/pending"
                }
            };

            var checkoutResponse = await _paymentService.CreateCheckoutPreferenceAsync(checkoutRequest);

            // 🔹 Retornar venta con datos de checkout
            return new VentaResponseDto
            {
                OrderId = ventaGuardada.Id,
                ClientId = ventaGuardada.ClientId,
                CustomerEmail = ventaGuardada.CustomerEmail,
                CustomerName = ventaGuardada.CustomerName,
                CustomerLastname = ventaGuardada.CustomerLastname,
                Street = ventaGuardada.Street,
                Number = ventaGuardada.Number,
                Department = ventaGuardada.Department,
                Description = ventaGuardada.Description,
                City = ventaGuardada.City,
                Province = ventaGuardada.Province,
                PostalCode = ventaGuardada.PostalCode,
                ShippingMethod = ventaGuardada.ShippingMethod,
                ShippingCost = ventaGuardada.ShippingCost,
                PaymentMethod = ventaGuardada.PaymentMethod,
                Notes = ventaGuardada.Notes,
                ExternalReference = ventaGuardada.ExternalReference,
                Total = ventaGuardada.Total,
                InitPoint = checkoutResponse.InitPoint,
                SandboxInitPoint = checkoutResponse.SandboxInitPoint,
                Items = ventaGuardada.DetalleVentas.Select(dv => new VentaItemDto
                {
                    ProductId = dv.ProductId,
                    Quantity = dv.Quantity,
                    UnitPrice = dv.UnitPrice,
                    Subtotal = dv.Subtotal,
                    ProductName = products[dv.ProductId].Name
                }).ToList()
            };
        }



        public void UpdateStatus(int id, VentaStatus newStatus)
        {
            var venta = _ventaRepository.Get(id) ?? throw new Exception("Venta no encontrada");

            if (venta.Status == VentaStatus.Cancelado)
                throw new Exception("No se puede cambiar el estado de una venta cancelada.");

            venta.Status = newStatus;
            _ventaRepository.Update(venta);
        }

        public List<VentaResponseDto> GetAllByClientOrEmail(int? clientId, string email)
        {
            var ventas = _ventaRepository.GetAll()
                .Where(v => (clientId != null && v.ClientId == clientId) || v.CustomerEmail == email)
                .ToList();

            return ventas.Select(MapToDto).ToList();
        }

        public async Task<VentaResponseDto?> GetByExternalReferenceAsync(string externalReference)
        {
            var venta = await _ventaRepository.GetByExternalReferenceAsync(externalReference);
            return venta == null ? null : MapToDto(venta);
        }

        public Venta GetEntityById(int id)
        {
            return _ventaRepository.GetById(id);
        }

        public void SetExternalReference(int id, string externalReference)
        {
            var venta = _ventaRepository.Get(id) ?? throw new Exception($"Venta con ID {id} no encontrada.");
            venta.ExternalReference = externalReference;
            _ventaRepository.Update(venta);
        }

        public async Task<Venta> GetEntityByIdAsync(int id, bool includeDetails = false)
        {
            if (includeDetails)
            {
                return await _ventaRepository.GetByIdWithDetailsAsync(id);
            }

            return _ventaRepository.GetById(id);
        }
    }
}
