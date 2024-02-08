namespace Yaginx.Models.TrafficModels
{
    public class HostTrafficListItem
    {
        public long Id { get; set; }
        public string HostName { get; set; }
        public long Period { get; set; }
        public long RequestCounts { get; set; }
        public long InboundBytes { get; set; }
        public string InboundBytesHuman => InboundBytes.GetByteHumanString();
        public long OutboundBytes { get; set; }
        public string OutboundBytesHuman => OutboundBytes.GetByteHumanString();
    }
}
