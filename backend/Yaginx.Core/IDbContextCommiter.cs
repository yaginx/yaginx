namespace Yaginx;

public interface IDbContextCommiter
{
    bool IsDbContextCreated { get; set; }
    Task CommitAsync();
}
