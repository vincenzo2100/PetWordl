using PetWorld.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<ChatMessage> AddAsync(ChatMessage message);
        Task<List<ChatMessage>> GetAllAsync();
    }
}
