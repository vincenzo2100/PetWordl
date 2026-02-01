using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Application.DTOs
{
    public class ChatResponseDto
    {
        public string Answer { get; set; } = string.Empty;
        public int IterationCount { get; set; }
    }
}
