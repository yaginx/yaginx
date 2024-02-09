using AgileLabs.Storage.Mongo;
using Amazon.Runtime.Internal;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using Yaginx.DataStore.MongoStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.MongoStore.Repositories
{
    internal class MonitorInfoRepository : YaginxNoSqlBaseRepository<ResourceMonitorInfoEntity>, IMonitorInfoRepository
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
}
