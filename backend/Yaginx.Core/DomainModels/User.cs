namespace Yaginx.DomainModels;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
}
