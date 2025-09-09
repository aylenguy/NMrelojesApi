using Application.Interfaces;
using Application.Model;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    public class ShippingService : IShippingService
    {
        public List<ShippingOptionDto> Calculate(string postalCode)
        {
            var options = new List<ShippingOptionDto>();

            // 🔹 Validación: CP debe ser numérico de 4 dígitos
            if (string.IsNullOrWhiteSpace(postalCode) ||
                postalCode.Length != 4 ||
                !postalCode.All(char.IsDigit))
            {
                return options; // Devuelve lista vacía si CP inválido
            }

            // 🔹 Caso Rosario (2000)
            if (postalCode == "2000")
            {
                options.Add(new ShippingOptionDto
                {
                    Name = "Retiro en tienda",
                    Cost = 0,
                    Description = "Disponible en nuestra sucursal de Rosario",
                    EstimatedDays = 0
                });
                options.Add(new ShippingOptionDto
                {
                    Name = "Envío por cadetería",
                    Cost = 0,
                    Description = "Entrega en 24/48 horas dentro de Rosario",
                    EstimatedDays = 2
                });
                options.Add(new ShippingOptionDto
                {
                    Name = "Correo Argentino - Estándar",
                    Cost = 0,
                    Description = "Entrega en 3 a 6 días hábiles",
                    EstimatedDays = 6
                });
            }
            // 🔹 Otros CP que empiezan con "2" (Santa Fe)
            else if (postalCode.StartsWith("2"))
            {

                options.Add(new ShippingOptionDto
                {
                    Name = "Retiro en tienda",
                    Cost = 0,
                    Description = "Disponible en nuestra sucursal",
                    EstimatedDays = 0
                });

                options.Add(new ShippingOptionDto
                {
                    Name = "Correo Argentino - Estándar",
                    Cost = 0,
                    Description = "Entrega en 3 a 6 días hábiles",
                    EstimatedDays = 6
                });
               


            }
            // 🔹 Resto del país
            else
            {
                options.Add(new ShippingOptionDto
                {
                    Name = "Correo Argentino - Estándar",
                    Cost = 0,
                    Description = "Entrega en 3 a 6 días hábiles",
                    EstimatedDays = 10
                });

                options.Add(new ShippingOptionDto
                {
                    Name = "Retiro en tienda",
                    Cost = 0,
                    Description = "Disponible en nuestra sucursal",
                    EstimatedDays = 6
                });
            }

            return options;
        }
    }
}
