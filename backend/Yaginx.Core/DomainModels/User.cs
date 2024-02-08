using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaginx.DomainModels
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }

    public interface IUserRepository
    {
        int Count();
        User GetByEmail(string email);
        void Add(User user);
        void Update(User user);
    }
}
