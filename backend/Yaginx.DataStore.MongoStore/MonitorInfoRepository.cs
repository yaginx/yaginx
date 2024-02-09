using AgileLabs.Storage.Mongo;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.MongoStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.MongoStore
{
    public class MonitorInfoRepository : YaginxNoSqlBaseRepository<ResourceMonitorInfoEntity>, IMonitorInfoRepository, IAppNoSqlBaseRepository<ResourceMonitorInfoEntity>
    {
        private readonly IMapper _mapper;

        public MonitorInfoRepository(MongodbContext<MongodbSetting> mongoDatabase, IMapper mapper, ILogger<MonitorInfoRepository> logger) : base(mongoDatabase, logger)
        {
            _mapper = mapper;
        }

        public async Task AddAsync(ResourceMonitorInfo monitorInfoEntity)
        {
            var entity = _mapper.Map<ResourceMonitorInfoEntity>(monitorInfoEntity);
            await base.AddAsync(entity);
        }
    }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ResourceMonitorInfo, ResourceMonitorInfoEntity>()
                .ForMember(d => d.ResourceUuid, mo => mo.MapFrom(s => "yaginx"));
            CreateMap<MonitorInfo, MonitorInfoEntity>();
        }
    }
}
