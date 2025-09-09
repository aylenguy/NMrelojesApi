// Models/ShippingOptionDto.cs
using System;

namespace Application.Model
{
    public class ShippingOptionDto
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public int EstimatedDays { get; set; }
    }

}
