using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetWorld.Infrastracture.Services
{
    public class ChatService : IChatService
    {
        private readonly IAgentOrchestrator _agentOrchestrator;
        private readonly IMessageRepository _messageRepository;
        public ChatService(IMessageRepository messageRepository,IAgentOrchestrator agentOrchestrator)
        {
            _agentOrchestrator = agentOrchestrator;
            _messageRepository = messageRepository;
        }

        public async Task<List<ChatMessage>> GetHistoryAsync()
        {
            return await _messageRepository.GetAllAsync();
        }

        public async Task<ChatResponseDto> ProcessQuestionAsync(string question)
        {
            var (answer, iterations) = await _agentOrchestrator.ProcessWithWriterCriticAsync(question);
            var message = new ChatMessage
            {
                Question = question,
                Answer = answer,
                Iterations = iterations,
                CreatedAt = DateTime.Now,
            };
            await _messageRepository.AddAsync(message);

            return new ChatResponseDto
            {
                Answer = answer,
                IterationCount = iterations
            };
        }
    }
}
