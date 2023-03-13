using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Core.Azure
{
    public class TwinClient
    {
        protected DigitalTwinsClient _client;

        protected TwinClient(AzureConfig azureConfig)
        {
            var credential =
                new ClientSecretCredential(azureConfig.TenantID, azureConfig.ClientID, azureConfig.ClientSecret);

            _client = new DigitalTwinsClient(new Uri(azureConfig.ADTInstanceUrl), credential);
        }

        protected TwinClient(DigitalTwinsClient client)
        {
            _client = client;
        }

        protected async Task DeleteModelByIDAsync(string modelID)
        {
            AsyncPageable<DigitalTwinsModelData> models = _client.GetModelsAsync();
            await foreach (DigitalTwinsModelData model in models)
            {
                if (model.Id == modelID)
                {
                    await _client.DeleteModelAsync(modelID);
                    break;
                }
            }
        }

        protected async Task<List<BasicDigitalTwin>> GetTwinsAsync(string modelID)
        {
            string query = $"SELECT * FROM DIGITALTWINS WHERE IS_OF_MODEL('{modelID}')";

            var twinList = new List<BasicDigitalTwin>();

            AsyncPageable<BasicDigitalTwin> queryResult = _client.QueryAsync<BasicDigitalTwin>(query);

            await foreach (BasicDigitalTwin twin in queryResult)
            {
                twinList.Add(twin);
            }

            return twinList;

        }

        protected async Task<int> GetTwinsCountAsync(string modelID)
        {
            int returnCount = 0;
            string query = $"SELECT COUNT() FROM DIGITALTWINS WHERE IS_OF_MODEL('{modelID}')";

            AsyncPageable<Dictionary<string, int>> queryResult = _client.QueryAsync<Dictionary<string, int>>(query);
            await foreach (var item in queryResult)
            {
                returnCount = Convert.ToInt32(item["COUNT"]);
            }

            return returnCount;
        }


        protected async Task<BasicDigitalTwin?> GetTwinByIDAsync(string twinID)
        {
            try
            {
                Response<BasicDigitalTwin> response = await _client.GetDigitalTwinAsync<BasicDigitalTwin>(twinID);
                return response.Value;
            }
            catch (RequestFailedException ex)
            {
                // If failed, check if it is because of not found error (code 404)
                if (ex.Status == 404)
                {
                    // Return null if the twin does not exist
                    return null;
                }
                else
                {
                    // Rethrow exception if it is for any other reason
                    throw;
                }
            }
        }

        protected async Task DeleteAllTwinsAsync(string modelID)
        {
            List<BasicDigitalTwin> twinList = await GetTwinsAsync(modelID);
            foreach (var twin in twinList)
            {
                await DeleteTwinAsync(twin.Id);
            }
        }

        protected async Task DeleteTwinAsync(string twinID)
        {
            // Delete all source relationship
            AsyncPageable<BasicRelationship> relationships = _client.GetRelationshipsAsync<BasicRelationship>(twinID);
            await foreach (BasicRelationship relationship in relationships)
            {
                await _client.DeleteRelationshipAsync(twinID, relationship.Id);
            }

            // Delete all incoming relationship
            AsyncPageable<IncomingRelationship> incomingRelationships = _client.GetIncomingRelationshipsAsync(twinID);
            await foreach (IncomingRelationship incomingRelationship in incomingRelationships)
            {
                await _client.DeleteRelationshipAsync(incomingRelationship.SourceId, incomingRelationship.RelationshipId);
            }

            await _client.DeleteDigitalTwinAsync(twinID);
        }
    }
}
