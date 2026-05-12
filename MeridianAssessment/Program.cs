using MeridianAssessment;
using MeridianAssessment.Handlers;
using MeridianAssessment.Interfaces;
using System.Net.Http;
using System.Text.Json;

try
{
    Console.WriteLine("Hello, World!");
    var client = new MeridianClient();

    Console.WriteLine("Level 1 — byte-stable fetch and submit");
    var levelOne = new LevelOneHandler(client);
    var levelOneResult = await levelOne.ProcessAsync();
}
catch (HttpRequestException httpEx)
{
    Console.WriteLine($"Network error occurred: {httpEx.StatusCode}");
}
catch (JsonException jsonEx)
{
    Console.WriteLine($"Data format error: {jsonEx.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
}
