namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch
{
    [Serializable]
    internal class WoScmBizException : Exception
    {
        public WoScmBizException()
        {
        }

        public WoScmBizException(string message) : base(message)
        {
        }

        public WoScmBizException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}