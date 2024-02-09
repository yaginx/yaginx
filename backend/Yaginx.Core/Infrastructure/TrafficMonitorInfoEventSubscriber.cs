using AgileLabs;
using AutoMapper;
using Yaginx.DomainModels.MonitorModels;
using Yaginx.MemoryBuses;

namespace Yaginx.Infrastructure;

public class TrafficMonitorInfoEventSubscriber : IEventSubscriber<MonitorMessage>
{
    private readonly IMapper _mapper;
    private readonly IMonitorInfoRepository _monitorInfoRepository;

    public TrafficMonitorInfoEventSubscriber(IMapper mapper, IMonitorInfoRepository monitorInfoRepository)
    {
        _mapper = mapper;
        _monitorInfoRepository = monitorInfoRepository;
    }

    public async Task Handle(MonitorMessage message)
    {
        await CreateMonitorInfoAsync(message);
    }

    internal async Task CreateMonitorInfoAsync(MonitorMessage message)
    {
        var dataInfo = _mapper.Map<List<MonitorInfo>>(message.data);

        var monitorInfoEntity = new ResourceMonitorInfo()
        {
            Timestamp = message.ts.FromEpochSeconds(),
            Data = dataInfo
        };
        await _monitorInfoRepository.AddAsync(monitorInfoEntity);
    }
}