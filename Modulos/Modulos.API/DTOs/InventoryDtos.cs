using System.ComponentModel.DataAnnotations;

namespace Modulos.API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Features { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int TotalStock { get; set; }
    }

    public class CreateProductRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Brand { get; set; } = string.Empty;
        public string? Features { get; set; }
        [Range(0, 1000000)]
        public decimal Price { get; set; }
    }

    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateWarehouseRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;
    }

    public class StockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class MovementRequest
    {
        [Required]
        public string Type { get; set; } = string.Empty; // Purchase, Sale, Transfer, Adjustment
        public int? FromWarehouseId { get; set; }
        public int? ToWarehouseId { get; set; }
        public string? Observations { get; set; }
        [Required]
        public List<MovementItemRequest> Items { get; set; } = new();
    }

    public class MovementItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
