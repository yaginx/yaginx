namespace AgileLabs.EfCore.PostgreSQL.DynamicSearch;

public class PaginationQueryWarapper<T>
{
    public PaginationQueryWarapper()
    {

    }

    public PaginationQueryWarapper(IQueryable<T> query)
    {
        Query = query;
    }
    public PaginationQueryWarapper(int count)
    {
        Count = count;
    }
    public PaginationQueryWarapper(IQueryable<T> query, int count)
    {
        Query = query;
        Count = count;
    }

    public IQueryable<T> Query { get; set; } = new List<T>().AsQueryable();
    public int Count { get; set; }

    public async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return await Query.SafeToListAsync(cancellationToken);
    }
}