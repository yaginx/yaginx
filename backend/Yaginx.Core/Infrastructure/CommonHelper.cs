using AgileLabs.FileProviders;
namespace Yaginx.Infrastructure;
public static class CommonHelper
{
	#region Properties

	/// <summary>
	/// Gets or sets the default file provider
	/// </summary>
	public static IAppFileProvider DefaultFileProvider { get; set; }

	#endregion
}