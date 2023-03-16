using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Core.Azure
{
    public class SectionClient : TwinClient
    {
        private string _sectionID = "section-1";
        private SectionModel? _sectionModelCache;
        private BasicDigitalTwin? _sectionTwinCache;

        public SectionClient(DigitalTwinsClient client) : base(client) { }

        public async Task<SectionModel> CreateSectionTwinAsync(SectionModel sectionModel)
        {
            sectionModel.SectionID = _sectionID;
            var sectionTwin = Mapper.MapSection(sectionModel);
            await _client.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(sectionTwin.Id, sectionTwin);
            return sectionModel;
        }
        
        public async Task<(SectionModel SectionModel, BasicDigitalTwin SectionTwin)> GetSectionAsync()
        {
            // checking whether section Twin exists
            if(_sectionTwinCache == null || _sectionModelCache == null) {
                _sectionTwinCache = await GetTwinByIDAsync(_sectionID);
                _sectionModelCache = Mapper.MapSection(_sectionTwinCache!);
            }
            return (_sectionModelCache!, _sectionTwinCache!);
        }
    }
}
