namespace Yaginx.YaginxAcmeLoaders
{
    public interface ICertificateDomainRepsitory
    {
        IEnumerable<string> GetFreeCertDomain();
        void UpdateDomainStatus(string domain, string message);
    }
}
