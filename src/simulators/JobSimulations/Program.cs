

using JobSimulations;
using System.Web;

Console.WriteLine("Please enter Azure DevOps org name (Just the org name, not the complete uri):");
var azdoUri = $"https://dev.azure.com/{Console.ReadLine()}" ;

Console.WriteLine("Please enter Azure DevOps PAT: ");
var azdoPat = Console.ReadLine();

Console.WriteLine("Please enter Azure DevOps Project Name: ");
var projectName = HttpUtility.UrlEncode(Console.ReadLine());

Console.WriteLine("Please enter Azure DevOps pipeline ID (you will find it in AzDO pipeline URI): ");
Int32.TryParse(Console.ReadLine(), out var pipelineId);

#pragma warning disable CS8604 // Possible null reference argument.
var pipelineAgent = new PipelineAgent(azdoUri, azdoPat);
#pragma warning restore CS8604 // Possible null reference argument.

Console.WriteLine("How many users to simulate? (ex: 5)");
Int32.TryParse(Console.ReadLine(), out var userCount);

var actors = new List<Actor>();
for(var i = 0; i < userCount; i++)
{
#pragma warning disable CS8604 // Possible null reference argument.
    var actor = new Actor(pipelineAgent, projectName, pipelineId);
#pragma warning restore CS8604 // Possible null reference argument.
    actors.Add(actor);
    await actor.GenerateLoadAsync();
}

Console.ReadLine();
