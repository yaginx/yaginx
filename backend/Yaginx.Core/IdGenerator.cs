using Snowflake.Core;

namespace Yaginx;

public class IdGenerator
{
    private static IdWorker worker = new IdWorker(1, 1);
    public static long NextId()
    {
        return worker.NextId();
    }
}
