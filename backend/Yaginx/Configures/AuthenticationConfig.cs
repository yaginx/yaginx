using AgileLabs.Utils;
using Yaginx.Infrastructure.Configuration;
/// <summary>
/// Represents hosting configuration parameters
/// </summary>
public partial class AuthenticationConfig : IFileConfig
{
	/// <summary>
	/// Gets or sets a value indicating whether to use proxy servers and load balancers
	/// </summary>
	public bool UseSecurity { get; private set; } = true;

	/// <summary>
	/// Gets or sets the header used to retrieve the value for the originating scheme (HTTP/HTTPS)
	/// </summary>
	public string UserName { get; private set; } = "yaginx";

	/// <summary>
	/// Gets or sets the header used to retrieve the originating client IP
	/// </summary>
	public string Password { get; private set; } = GuidGenerator.GenerateDigitalUUID();
}