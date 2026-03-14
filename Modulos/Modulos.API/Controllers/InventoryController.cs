using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modulos.API.Data;
using Modulos.API.DTOs;
using Modulos.API.Models;
using System.Security.Claims;

namespace Modulos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Products ---

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            return await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Brand = p.Brand,
                    Features = p.Features,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    TotalStock = p.Stocks.Sum(s => s.Quantity)
                }).ToListAsync();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("products")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Brand = request.Brand,
                Features = request.Features,
                Price = request.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new ProductDto { Id = product.Id, Name = product.Name });
        }

        // --- Warehouses ---

        [HttpGet("warehouses")]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetWarehouses()
        {
            return await _context.Warehouses
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    IsActive = w.IsActive
                }).ToListAsync();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("warehouses")]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse(CreateWarehouseRequest request)
        {
            var warehouse = new Warehouse
            {
                Name = request.Name,
                Location = request.Location
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return Ok(new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name });
        }

        // --- Movements ---

        [HttpGet("movements")]
        public async Task<ActionResult<IEnumerable<Movement>>> GetMovements()
        {
            return await _context.Movements
                .Include(m => m.FromWarehouse)
                .Include(m => m.ToWarehouse)
                .Include(m => m.User)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("movements")]
        public async Task<IActionResult> RecordMovement(MovementRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var type = Enum.Parse<MovementType>(request.Type, true);
                var movement = new Movement
                {
                    Type = type,
                    FromWarehouseId = request.FromWarehouseId,
                    ToWarehouseId = request.ToWarehouseId,
                    Observations = request.Observations,
                    UserId = userId,
                    Date = DateTime.UtcNow
                };

                foreach (var item in request.Items)
                {
                    movement.Details.Add(new MovementDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });

                    // Update Stocks
                    if (request.FromWarehouseId.HasValue)
                    {
                        var stock = await _context.Stocks.FindAsync(item.ProductId, request.FromWarehouseId.Value);
                        if (stock == null || stock.Quantity < item.Quantity)
                            return BadRequest($"Stock insuficiente para el producto {item.ProductId} en el almacén de origen.");
                        
                        stock.Quantity -= item.Quantity;
                    }

                    if (request.ToWarehouseId.HasValue)
                    {
                        var stock = await _context.Stocks.FindAsync(item.ProductId, request.ToWarehouseId.Value);
                        if (stock == null)
                        {
                            stock = new ProductWarehouse { ProductId = item.ProductId, WarehouseId = request.ToWarehouseId.Value, Quantity = item.Quantity };
                            _context.Stocks.Add(stock);
                        }
                        else
                        {
                            stock.Quantity += item.Quantity;
                        }
                    }
                }

                _context.Movements.Add(movement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Movimiento registrado exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("Error al procesar el movimiento: " + ex.Message);
            }
        }
    }
}
