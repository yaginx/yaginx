using System.Diagnostics;

namespace Yaginx.SelfManagement.CustomEndpoints
{
    internal static class Observability
    {
        public static readonly ActivitySource ManagedApiActivitySource = new ActivitySource("QuantumCorp.ApiGate.ManagedApi");

        public static Activity GetManagedApiActivity(this HttpContext context)
        {
            return context.Features[typeof(ManagedApiActivity)] as Activity;
        }

        public static void SetManagedApiActivity(this HttpContext context, Activity activity)
        {
            if (activity is not null)
            {
                context.Features[typeof(ManagedApiActivity)] = activity;
            }
        }

        public static void AddError(this Activity activity, string message, string description)
        {
            if (activity is not null)
            {
                var tagsCollection = new ActivityTagsCollection
            {
                { "error", message },
                { "description", description }
            };

                activity.AddEvent(new ActivityEvent("Error", default, tagsCollection));
            }
        }

        private class ManagedApiActivity
        {
        }
    }
}
