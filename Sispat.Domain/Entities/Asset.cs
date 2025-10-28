using Sispat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Domain.Entities
{
    public class Asset
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string? Description { get; set; }

        
        public string SerialNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseValue { get; set; }
        public DateTime PurchaseDate { get; set; }
        public AssetStatus Status { get; set; }
                
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
                
        public Guid? LocationId { get; set; }
        public virtual Location? Location { get; set; }
               
        public string? AssignedToUserId { get; set; }
        public virtual ApplicationUser? AssignedToUser { get; set; }
    }
}
