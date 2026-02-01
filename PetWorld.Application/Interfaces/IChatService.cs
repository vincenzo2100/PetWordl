using PetWorld.Application.DTOs;
using PetWorld.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponseDto> ProcessQuestionAsync(string question);
        Task<List<ChatMessage>> GetHistoryAsync();
    }
}
