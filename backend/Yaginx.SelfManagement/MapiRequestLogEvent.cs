namespace Yaginx.SelfManagement
{
    internal class MapiRequestLogEvent
    {
        public MapiRequestLogEvent()
        {
        }

        public string Api { get; set; }
        public DateTime RequestTime { get; set; }
        public long AccountId { get; set; }
    }
}