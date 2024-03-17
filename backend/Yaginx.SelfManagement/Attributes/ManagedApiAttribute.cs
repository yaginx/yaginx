namespace Yaginx.SelfManagement.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ManagedApiAttribute : Attribute
    {
        public string Url { get; set; }
    }
}
