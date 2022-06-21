

namespace JobSimulations
{
    public class Actor
    {
        private readonly PipelineAgent pipelineAgent;
        private readonly string projectName;
        private readonly int pipelineId;
        private Timer timerInstance;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Actor(PipelineAgent pipelineAgent, string projectName, int pipelineId)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.pipelineAgent = pipelineAgent;
            this.projectName = projectName;
            this.pipelineId = pipelineId;            
        }

        public async Task GenerateLoadAsync()
        {
            timerInstance = new Timer(async _ =>
                {
                    await this.pipelineAgent.RunPipelineAsync(projectName, pipelineId);
                },
                null,
                new Random(DateTime.Now.Millisecond).Next(5 * 1000),
                new Random(DateTime.Now.Millisecond).Next(5 * 1000));
            await Task.CompletedTask;
        }
    }
}
