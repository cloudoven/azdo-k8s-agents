

using AgentController.AzDO;
using AgentController.AzDO.Supports;

namespace JobSimulations
{
    public class PipelineAgent : AgentService
    {
        public PipelineAgent(string adoUrl, string pat) : base(adoUrl, pat)
        {
        }

        public async Task RunPipelineAsync(string? projectName, int pipelineId)
        {   
            var path = $"{OrgName}/{projectName}/_apis/pipelines/{pipelineId}/runs?api-version=7.1-preview.1";
            
            await Http.PostRestAsync(path, new 
            {
                stagesToSkip = new List<object> { },
                variables = new { },
                resources = new
                {
                    repositories = new 
                    {
                        self = new 
                        {
                            refName = "refs/heads/main"
                        }
                    }
                }
            });
        }
    }
}
