using System.ComponentModel.DataAnnotations;

namespace Library.ControllerApi.DTOs;

public record InventoryCreateDto(
    [Required, MaxLength(20)] string sku,
    [Required, MaxLength(200)] string Name,
    [Required, Range(0.01, 100000)] decimal price,
    [Required, Range(0, 1000)] int stock
);