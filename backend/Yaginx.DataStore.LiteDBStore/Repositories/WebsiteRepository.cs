using AutoMapper;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.LiteDBStore.Repositories
{
    public class WebsiteRepository : IWebsiteRepository
    {
        private readonly LiteDbDatabaseRepository _databaseRepository;
        private readonly IMapper _mapper;

        public WebsiteRepository(LiteDbDatabaseRepository databaseRepository, IMapper mapper)
        {
            _databaseRepository = databaseRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(WebsiteDomainModel website)
        {
            await _databaseRepository.InsertAsync(website);
        }

        public async Task DeleteAsync(long id)
        {
            await _databaseRepository.DeleteAsync<WebsiteDomainModel>(id);
        }

        public Task<WebsiteDomainModel> GetAsync(long id)
        {
            return _databaseRepository.GetAsync<WebsiteDomainModel>(id);
        }

        public async Task<WebsiteDomainModel> GetByNameAsync(string name)
        {
            return await _databaseRepository.GetAsync<WebsiteDomainModel>(x => x.Name == name);
        }

        public async Task<IEnumerable<WebsiteDomainModel>> SearchAsync()
        {
            var websiteList = await _databaseRepository.SearchAsync<Website>();
            return _mapper.Map<List<WebsiteDomainModel>>(websiteList);
        }

        public async Task UpdateAsync(WebsiteDomainModel website)
        {
            await _databaseRepository.UpdateAsync(website.Id, website);
        }
    }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Website, WebsiteDomainModel>()
                 .ForMember(d => d.Specifications, mo => mo.MapFrom(s => new WebsiteSpecifications
                 {
                     DefaultHost = s.DefaultHost,
                     DefaultDestination = s.DefaultDestination,
                     DefaultDestinationHost = s.DefaultDestinationHost,
                     IsAllowUnsafeSslCertificate = s.IsAllowUnsafeSslCertificate,
                     IsAutoRedirectHttp2Https = s.IsAutoRedirectHttp2Https,
                     IsWithOriginalHostHeader = s.IsWithOriginalHostHeader,
                     WebProxy = s.WebProxy
                 }));
        }
    }

    public class Website : WebsiteSpecifications
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        public string[] Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Dictionary<string, string> ProxyTransforms { get; set; }
        public SimpleResponseItem[] SimpleResponses { get; set; }
    }
}
