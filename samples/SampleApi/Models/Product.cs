using System;
using System.ComponentModel.DataAnnotations;

namespace SampleApi.Models
{
    /// <summary>
    /// Product entity for demonstration
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        [MaxLength(50)]
        public string? Category { get; set; }

        /// <summary>
        /// Whether the product is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When the product was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the product was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// When the product was last updated
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}


