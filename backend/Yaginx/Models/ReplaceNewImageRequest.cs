namespace Yaginx.Models
{
	public class ReplaceNewImageRequest
	{
		public string Name { get; set; }
		public string Image { get; set; }
		public List<string> Envs { get; set; }
		public List<KeyValuePair<string, List<string>>> Ports { get; set; }
		public List<string> Volumns { get; set; }
		public List<string> Links { get; set; }
		public List<string> Hosts { get; set; }
	}
}
