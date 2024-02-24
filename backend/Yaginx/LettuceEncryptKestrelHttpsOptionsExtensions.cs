using McMaster.AspNetCore.Kestrel.Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Yaginx;

/// <summary>
/// Methods for configuring Kestrel.
/// </summary>
public static class LettuceEncryptKestrelHttpsOptionsExtensions
{
    private const string MissingServicesMessage =
        "Missing required LettuceEncrypt services. Did you call '.AddLettuceEncrypt()' to add these your DI container?";

    /// <summary>
    /// Configured LettuceEncrypt on this HTTPS endpoint for Kestrel.
    /// </summary>
    /// <param name="httpsOptions">Kestrel's HTTPS configuration.</param>
    /// <param name="applicationServices"></param>
    /// <returns>The original HTTPS options with some required settings added to it.</returns>
    /// <exception cref="InvalidOperationException">
    /// Raised if <see cref="LettuceEncryptServiceCollectionExtensions.AddLettuceEncrypt(IServiceCollection)"/>
    /// has not been used to add required services to the application service provider.
    /// </exception>
    public static HttpsConnectionAdapterOptions UseYaginxLettuceEncrypt(
        this HttpsConnectionAdapterOptions httpsOptions,
        IServiceProvider applicationServices)
    {
        var selector = applicationServices.GetService<IServerCertificateSelector>();

        if (selector is null)
        {
            throw new InvalidOperationException(MissingServicesMessage);
        }

        return httpsOptions.UseLettuceEncrypt(selector);
    }

    internal static HttpsConnectionAdapterOptions UseLettuceEncrypt(
        this HttpsConnectionAdapterOptions httpsOptions,
        IServerCertificateSelector selector)
    {
        httpsOptions.UseServerCertificateSelector(selector);
        return httpsOptions;
    }
}
