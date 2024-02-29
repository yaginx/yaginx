namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public interface IDbContextCommiter
    {
        bool IsDbContextCreated { get; set; }
        Task CommitAsync();
    }
}
