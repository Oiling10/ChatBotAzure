using Azure;
using Azure.AI.Language.QuestionAnswering;
using System;
using System.Threading.Tasks;

public class QuestionAnsweringService
{
    private readonly QuestionAnsweringClient _client;

    public QuestionAnsweringService(string endpoint, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key or Endpoint is not configured properly.");
        }

        Console.WriteLine($"Endpoint: {endpoint}");
        Console.WriteLine($"API Key: {apiKey}");

        _client = new QuestionAnsweringClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    public async Task AskQuestionAsync(string projectName, string deploymentName, string question)
    {
        QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);
        Response<AnswersResult> response = await _client.GetAnswersAsync(question, project);

        foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
        {
            Console.WriteLine($"({answer.Confidence:P2}) {answer.Answer}");
            Console.WriteLine($"Source: {answer.Source}");
            Console.WriteLine();
        }
    }
}
