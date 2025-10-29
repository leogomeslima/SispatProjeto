using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sispat.Application.DTOs
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Building { get; set; }
        public string? Floor { get; set; }
    }

    // DTO para criar ou atualizar uma localização
    public class CreateOrUpdateLocationDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Building { get; set; }
        public string? Floor { get; set; }
    }
}
