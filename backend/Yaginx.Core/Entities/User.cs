using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaginx.Entities
{
	public class User
	{
		public long Id { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
	}
}
