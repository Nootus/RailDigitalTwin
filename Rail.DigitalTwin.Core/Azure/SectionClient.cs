using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.Azure
{
    public class SectionClient : TwinClient
    {
        private string _sectionID = "section-1";

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
            var sectionTwin = await GetTwinByIDAsync(_sectionID);
            SectionModel sectionModel = Mapper.MapSection(sectionTwin!);
            return (sectionModel, sectionTwin!);
        }

    }
}
