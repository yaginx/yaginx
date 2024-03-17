namespace Yaginx.SelfManagement.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagedApiServiceAttribute : Attribute
    {
        public string Brand { get; set; }
        public string Group { get; set; }
        public string ServiceName { get; set; }
    }
}
