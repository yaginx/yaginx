using System.Diagnostics;
namespace Yaginx.SimpleProcessors;

internal static class Observability
{
    public static readonly ActivitySource SimpleProcessorActivitySource = new ActivitySource("Yaginx.SimpleProcessor");

    public static Activity? GetYarpActivity(this HttpContext context)
    {
        return context.Features[typeof(YaginxActivity)] as Activity;
    }

    public static void SetYarpActivity(this HttpContext context, Activity? activity)
    {
        if (activity is not null)
        {
            context.Features[typeof(YaginxActivity)] = activity;
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

    private class YaginxActivity
    {
    }
}

