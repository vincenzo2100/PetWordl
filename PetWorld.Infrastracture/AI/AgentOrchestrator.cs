using Anthropic;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Anthropic;
using Microsoft.Extensions.Configuration;
using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using System.Text.Json;

namespace PetWorld.Infrastructure.AI;

public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly AIAgent _writerAgent;
    private readonly AIAgent _criticAgent;
    private const int MaxIterations = 3;

    private const string ProductCatalog = @"
Product Catalog for PetWorld:

1. Royal Canin Adult Dog 15kg - Karma dla psów - 289 zł - Premium karma dla dorosłych psów średnich ras
2. Whiskas Adult Kurczak 7kg - Karma dla kotów - 129 zł - Sucha karma dla dorosłych kotów z kurczakiem
3. Tetra AquaSafe 500ml - Akwarystyka - 45 zł - Uzdatniacz wody do akwarium, neutralizuje chlor
4. Trixie Drapak XL 150cm - Akcesoria dla kotów - 399 zł - Wysoki drapak z platformami i domkiem
5. Kong Classic Large - Zabawki dla psów - 69 zł - Wytrzymała zabawka do napełniania smakołykami
6. Ferplast Klatka dla chomika - Gryzonie - 189 zł - Klatka 60x40cm z wyposażeniem
7. Flexi Smycz automatyczna 8m - Akcesoria dla psów - 119 zł - Smycz zwijana dla psów do 50kg
8. Brit Premium Kitten 8kg - Karma dla kotów - 159 zł - Karma dla kociąt do 12 miesiąca życia
9. JBL ProFlora CO2 Set - Akwarystyka - 549 zł - Kompletny zestaw CO2 dla roślin akwariowych
10. Vitapol Siano dla królików 1kg - Gryzonie - 25 zł - Naturalne siano łąkowe, podstawa diety
";

    private const string WriterInstructions = @"You are a helpful and friendly pet store assistant for PetWorld, an online pet shop.

Your responsibilities:
- Answer customer questions about pet products
- Recommend products from the catalog based on their needs
- Provide helpful advice about pet care
- Be warm, professional, and enthusiastic

" + ProductCatalog + @"

Guidelines:
- Always be friendly and helpful
- Recommend specific products when relevant, including their names and prices
- If you recommend a product, explain why it's suitable
- If the customer's question is unclear, ask clarifying questions
- Keep responses concise but informative
- Use Polish language naturally when mentioning product names and prices";

    private const string CriticInstructions = @"You are a quality control critic for PetWorld customer service responses.

Your job is to evaluate whether the Writer's response is:
1. Helpful and relevant to the customer's question
2. Accurate (product recommendations match the catalog and customer needs)
3. Friendly and professional in tone
4. Complete (answers the question fully)
5. Includes specific product names and prices when making recommendations

Respond ONLY in this JSON format (no markdown, no extra text):
{
  ""approved"": true or false,
  ""feedback"": ""specific improvements needed if not approved, or empty string if approved""
}

Be strict but fair. Only approve responses that genuinely help the customer.";

    public AgentOrchestrator(IConfiguration configuration)
    {
        var apiKey = configuration["Anthropic:ApiKey"]
                     ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
                     ?? throw new InvalidOperationException("Anthropic API key not found");

        var deploymentName = configuration["Anthropic:DeploymentName"]
                             ?? Environment.GetEnvironmentVariable("ANTHROPIC_DEPLOYMENT_NAME")
                             ?? "claude-3-5-sonnet-20241022";

        // Create Anthropic client using Microsoft Agent Framework
        var anthropicClient = new AnthropicClient()
        {
            APIKey = apiKey
        };

        // Create Writer Agent using Microsoft Agent Framework
        _writerAgent = anthropicClient.AsAIAgent(
            model: deploymentName,
            name: "WriterAgent",
            instructions: WriterInstructions
        );

        // Create Critic Agent using Microsoft Agent Framework
        _criticAgent = anthropicClient.AsAIAgent(
            model: deploymentName,
            name: "CriticAgent",
            instructions: CriticInstructions
        );
    }

    public async Task<(string answer, int iterations)> ProcessWithWriterCriticAsync(string question)
    {
        string currentResponse = string.Empty;
        string feedbackContext = string.Empty;
        int iteration = 0;

        for (iteration = 1; iteration <= MaxIterations; iteration++)
        {
            // Writer generates response
            currentResponse = await GenerateWriterResponseAsync(question, feedbackContext);

            // Critic evaluates response
            var critique = await EvaluateCriticAsync(question, currentResponse);

            if (critique.Approved || iteration == MaxIterations)
            {
                break;
            }

            // Prepare feedback for next iteration
            feedbackContext = $"\n\nPrevious attempt needs improvement. Feedback: {critique.Feedback}\nPlease address these issues and provide an improved response.";
        }

        return (currentResponse, iteration);
    }

    private async Task<string> GenerateWriterResponseAsync(string question, string feedbackContext)
    {
        try
        {
            var prompt = question + feedbackContext;
            var result = await _writerAgent.RunAsync(prompt);
            return result.Text ?? "I apologize, but I couldn't generate a response. Please try again.";
        }
        catch (Exception ex)
        {
            return $"I apologize, but I encountered an error: {ex.Message}. Please try again.";
        }
    }

    private async Task<CriticResponseDto> EvaluateCriticAsync(string question, string response)
    {
        try
        {
            var prompt = $"Customer question: {question}\n\nWriter's response: {response}\n\nEvaluate this response.";
            var result = await _criticAgent.RunAsync(prompt);
            var jsonResponse = result.Text ?? "{}";

            // Clean up potential markdown formatting
            jsonResponse = jsonResponse.Trim().Replace("```json", "").Replace("```", "").Trim();

            var critique = JsonSerializer.Deserialize<CriticResponseDto>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return critique ?? new CriticResponseDto { Approved = true, Feedback = "" };
        }
        catch
        {
            // If parsing fails, approve to avoid infinite loops
            return new CriticResponseDto { Approved = true, Feedback = "" };
        }
    }
}