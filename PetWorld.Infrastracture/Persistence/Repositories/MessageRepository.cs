using Microsoft.EntityFrameworkCore;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Infrastracture.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly PetWorldDbContext _context;

        public MessageRepository(PetWorldDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage> AddAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<ChatMessage>> GetAllAsync()
        {
            return await _context.ChatMessages.OrderByDescending(m => m.CreatedAt).ToListAsync();
        }
    }
}
