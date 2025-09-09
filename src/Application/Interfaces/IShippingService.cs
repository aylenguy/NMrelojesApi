using Application.Model;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IShippingService
    {
        /// <summary>
        /// Calcula las opciones de envío disponibles en base al código postal.
        /// </summary>
        /// <param name="postalCode">Código postal (4 dígitos numéricos).</param>
        /// <returns>Lista de opciones de envío disponibles.</returns>
        List<ShippingOptionDto> Calculate(string postalCode);
    }
}
