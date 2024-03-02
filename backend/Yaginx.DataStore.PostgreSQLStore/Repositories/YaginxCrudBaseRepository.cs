using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Abstracted.ContextFactories;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public abstract class YaginxCrudBaseRepository<TDomainModel, TEntity> : CrudRepository<TEntity>
        where TEntity : class, new()
        where TDomainModel : class, new()
    {
        protected readonly IMapper _mapper;

        public YaginxCrudBaseRepository(IWoDbContextFactory factory, IMapper mapper, ILogger logger) : base(factory, logger)
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
