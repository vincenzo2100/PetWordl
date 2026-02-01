using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int Iterations { get; set; }
    }
}
