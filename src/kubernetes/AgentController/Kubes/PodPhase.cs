using k8s.Models;
using System;
using System.Collections.Generic;

namespace AgentController.Kubes
{
    // Gets or sets the phase of a Pod is a simple, high-level summary of where the
    // Pod is in its lifecycle. The conditions array, the reason and message fields,
    // and the individual container status arrays contain more detail about the pod's
    // status.
    // There are five possible phase values:
    //
    // Pending: The pod has been accepted
    // by the Kubernetes system, but one or more of the container images has not been
    // created. This includes time before being scheduled as well as time spent downloading
    // images over the network, which could take a while.
    //
    // Running: The pod has been
    // bound to a node, and all of the containers have been created. At least one container
    // is still running, or is in the process of starting or restarting.
    //
    //
    // Succeeded:
    // All containers in the pod have terminated in success, and will not be restarted.
    //
    //
    // Failed: All containers in the pod have terminated, and at least one container
    // has terminated in failure. The container either exited with non-zero status or
    // was terminated by the system.
    //
    // Unknown: For some reason the state of the pod could
    // not be obtained, typically due to an error in communicating with the host of
    // the pod. More info: https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle#pod-phase
    public static class PodPhase
    {
        public const string Pending = "Pending";
        public const string Running = "Running";
        public const string Succeeded = "Succeeded";
        public const string Failed = "Failed";

        public static bool IsInSuccededOrFailedPhase(this V1Pod pod)
        {
            return Succeeded.Equals(pod.Status.Phase, StringComparison.OrdinalIgnoreCase) ||
                Failed.Equals(pod.Status.Phase, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsActive(this V1Pod pod)
        {
            return Pending.Equals(pod.Status.Phase, StringComparison.OrdinalIgnoreCase) ||
                Running.Equals(pod.Status.Phase, StringComparison.OrdinalIgnoreCase);
        }

        public static Dictionary<string,string> ExtractLables(this IList<V1Pod> pods, string labelName)
        {
            var cache = new Dictionary<string, string>();
            foreach(var pod in pods)
            {
                var value = pod.GetLabel(labelName);
                if(!string.IsNullOrEmpty(value))
                {
                    cache.TryAdd(value, value);
                }
            }
            return cache;
        }
    }
}
