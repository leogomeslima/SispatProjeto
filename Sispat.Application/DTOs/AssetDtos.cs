using Sispat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.DTOs
{
    public class AssetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public decimal PurchaseValue { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty; // String, não Enum

        // Informações aninhadas (nested)
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public Guid? LocationId { get; set; }
        public string? LocationName { get; set; }

        public string? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
    }

    // DTO para CRIAR (POST) um novo ativo
    public class CreateAssetDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public decimal PurchaseValue { get; set; }
        public DateTime PurchaseDate { get; set; }
        public AssetStatus Status { get; set; } // O Enum é ok para entrada

        public Guid CategoryId { get; set; }
        public Guid? LocationId { get; set; }
        public string? AssignedToUserId { get; set; }
    }

    // DTO para ATUALIZAR (PUT) um ativo
    public class UpdateAssetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PurchaseValue { get; set; }
        public DateTime PurchaseDate { get; set; }
        public AssetStatus Status { get; set; }

        public Guid CategoryId { get; set; }
        public Guid? LocationId { get; set; }
        public string? AssignedToUserId { get; set; }
        // Note: SerialNumber (N° de Série) geralmente não deve ser editável.
    }
}
