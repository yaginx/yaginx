namespace AgileLabs.EfCore.PostgreSQL;

public interface IDbContextCommiter
{
    bool IsDbContextCreated { get; set; }
    Task CommitAsync();
}
