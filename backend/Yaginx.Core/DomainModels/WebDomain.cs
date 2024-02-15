using System.Threading.Tasks;

namespace Yaginx.DomainModels;

/// <summary>
/// 网站域名
/// </summary>
public class WebDomain
{
    public long? Id { get; set; }
    public string Name { get; set; }
    public bool IsUseFreeCert { get; set; }
    public bool IsVerified { get; set; }
    public string FreeCertMessage { get; set; }
}
