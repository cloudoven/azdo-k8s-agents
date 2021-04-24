using k8s;
using k8s.Models;

namespace AgentController.Kubes
{
    public static class PodEventWatcher
    {
        public static bool IsInSuccededOrFailedPhase(this V1Pod pod, WatchEventType eventType)
        {
            if(eventType == WatchEventType.Modified)
            {
                return pod.IsInSuccededOrFailedPhase();
            }
            return false;
        }
    }
}
