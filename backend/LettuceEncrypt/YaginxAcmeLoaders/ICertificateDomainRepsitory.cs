namespace Yaginx.YaginxAcmeLoaders
{
    public interface ICertificateDomainRepsitory
    {
        Task<IEnumerable<string>> GetFreeCertDomainAsync();
        Task UnFreeDomainAsync(string domain, string message);
    }
}
