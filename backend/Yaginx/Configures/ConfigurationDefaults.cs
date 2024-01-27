namespace Yaginx.Configures;

/// <summary>
/// Represents default values related to configuration services
/// </summary>
public static partial class ConfigurationDefaults
{
	/// <summary>
	/// Gets the path to file that contains app settings
	/// </summary>
	public static string AppSettingsFilePath => "config/appsettings.json";

	/// <summary>
	/// Gets the path to file that contains app settings for specific hosting environment
	/// </summary>
	/// <remarks>0 - Environment name</remarks>
	public static string AppSettingsEnvironmentFilePath => "config/appsettings.{0}.json";
}