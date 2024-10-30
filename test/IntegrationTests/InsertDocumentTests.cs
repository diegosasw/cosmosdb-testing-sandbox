using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Contracts;
using Sample.Api;
using Sample.Api.CosmosDb;
using Test.Common.Extensions;
using Test.Common.Fixtures;
using xRetry;
using Xunit.Abstractions;

namespace IntegrationTests;

public static class InsertDocumentTests
{
    [Trait("Category", "A")]
    public class GivenDatabaseAvailable(TestContainerFixture fixture, ITestOutputHelper output) 
        : IntegrationTestBase(fixture, output)
    {
        private CosmosDbDocument _document = null!;
        private HttpResponseMessage _result = null!;

        protected override Task Given()
        {
            var documentId = Guid.NewGuid().ToString();
            const string text = "foo";
            var createdOn = DateTime.UtcNow;
            
            _document = new CosmosDbDocument(documentId, text, createdOn);

            return Task.CompletedTask;
        }

        protected override async Task When()
        {
            _result = await HttpClient.PostAsJsonAsync("runsample", _document);
        }

        [RetryFact]
        public void ThenItShouldReturnSuccess()
        {
            _result.Should().BeSuccessful();
        }
        
        [RetryFact]
        public async Task ThenItShouldReturnExpectedResponse()
        {
            var commandHttpResponse = await _result.ToPayload<CommandHttpResponse>();
            commandHttpResponse.Should().NotBeNull();
        }
    }
}