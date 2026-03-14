using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modulos.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Brand { get; set; } = string.Empty;
        public string? Features { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public virtual ICollection<ProductWarehouse> Stocks { get; set; } = new List<ProductWarehouse>();
    }

    public class Warehouse
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Relationships
        public virtual ICollection<ProductWarehouse> Stocks { get; set; } = new List<ProductWarehouse>();
    }

    public class ProductWarehouse
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class Movement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public MovementType Type { get; set; }
        public int? FromWarehouseId { get; set; }
        public virtual Warehouse? FromWarehouse { get; set; }
        public int? ToWarehouseId { get; set; }
        public virtual Warehouse? ToWarehouse { get; set; }
        public string? Observations { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<MovementDetail> Details { get; set; } = new List<MovementDetail>();
    }

    public class MovementDetail
    {
        public int Id { get; set; }
        public int MovementId { get; set; }
        public virtual Movement Movement { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class ScheduledInventory
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;
        public string? Observations { get; set; }
        public InventoryStatus Status { get; set; }
    }

    public enum MovementType
    {
        Purchase = 0,
        Sale = 1,
        Transfer = 2,
        Adjustment = 3
    }

    public enum InventoryStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}
