using Snowflake.Core;

namespace Yaginx.DataStores
{
	public class IdGenerator
	{
		private static IdWorker worker = new Snowflake.Core.IdWorker(1, 1);
		public static long NextId()
		{
			return worker.NextId();
		}
	}
}
