using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Application.Interfaces
{
    public interface IAgentOrchestrator
    {
        Task<(string answer, int iterations)> ProcessWithWriterCriticAsync(string question);
    }

}
