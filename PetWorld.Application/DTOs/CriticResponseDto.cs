using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Application.DTOs
{
    public class CriticResponseDto
    {
        public bool Approved { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
