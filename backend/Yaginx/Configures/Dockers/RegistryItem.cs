namespace Yaginx.Configures.Dockers
{
	public class RegistryItem
	{
		public string ServerAddress { get; set; }
		public string UserName { get; private set; }
		public string Password { get; private set; }
	}
}
