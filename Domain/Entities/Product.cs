using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int CategoryId { get; set; }

        public Category? Category { get; set; } 
    }
}
