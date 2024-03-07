using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public abstract class YaginxCrudBaseRepository<TDomainModel, TEntity> : CrudRepository<TEntity>
        where TEntity : class, new()
        where TDomainModel : class, new()
    {
        protected readonly IMapper _mapper;

        public YaginxCrudBaseRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger logger) : base(factory, logger, mapper)
        {
            _mapper = mapper;
        }

        public async Task AddAsync(TDomainModel website)
        {
            await base.InsertAsync(_mapper.Map<TEntity>(website));
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await base.GetByIdAsync(id);
            await base.DeleteAsync(entity);
        }

        public async Task<TDomainModel> GetAsync(long id)
        {
            var entity = await base.GetByIdAsync(id);
            return _mapper.Map<TDomainModel>(entity);
        }

        public async Task<IEnumerable<TDomainModel>> SearchAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var entityList = await base.GetByQueryAsync(predicate);
            return _mapper.Map<List<TDomainModel>>(entityList);
        }

        public async Task UpdateAsync(TDomainModel website)
        {
            await base.EntryUpdateAsync(_mapper.Map<TEntity>(website));
        }

        public async Task<long> CountAsync()
        {
            return await base.CountAsync(x => true);
        }
    }
}
