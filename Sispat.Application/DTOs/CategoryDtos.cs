using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.DTOs
{
    public class CategoryDtos
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateOrUpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
