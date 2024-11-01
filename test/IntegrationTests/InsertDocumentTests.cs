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
    public class GivenDatabaseAvailableAndTheory(TestContainerFixture fixture, ITestOutputHelper output) 
        : IntegrationTestBase(fixture, output)
    {
        [RetryTheory]
        [InlineData("one")]
        [InlineData("two")]
        [InlineData("three")]
        [InlineData("four")]
        [InlineData("five")]
        [InlineData("six")]
        [InlineData("seven")]
        [InlineData("eight")]
        [InlineData("nine")]
        [InlineData("ten")]
        [InlineData("eleven")]
        [InlineData("twelve")]
        [InlineData("thirteen")]
        [InlineData("fourteen")]
        [InlineData("fifteen")]
        [InlineData("sixteen")]
        [InlineData("seventeen")]
        [InlineData("eighteen")]
        [InlineData("nineteen")]
        [InlineData("twenty")]
        public async Task Create_Container_Should_Succeed(string text)
        {
            var documentId = Guid.NewGuid().ToString();
            var createdOn = DateTime.UtcNow;
            var document = new CosmosDbDocument(documentId, text, createdOn);
            var response = await HttpClient.PostAsJsonAsync("runsample", document);
            var commandHttpResponse = await response.ToPayload<CommandHttpResponse>();

            response.Should().BeSuccessful();
            commandHttpResponse.Payload.Text.Should().Be(document.text);
        }
    }
    
    public class GivenDatabaseAvailableAndTheoryAndClearingContainers(TestContainerFixture fixture, ITestOutputHelper output) 
        : IntegrationTestBase(fixture, output, clearContainers: true)
    {
        [RetryTheory]
        [InlineData("one")]
        [InlineData("two")]
        [InlineData("three")]
        [InlineData("four")]
        [InlineData("five")]
        [InlineData("six")]
        [InlineData("seven")]
        [InlineData("eight")]
        [InlineData("nine")]
        [InlineData("ten")]
        [InlineData("eleven")]
        [InlineData("twelve")]
        [InlineData("thirteen")]
        [InlineData("fourteen")]
        [InlineData("fifteen")]
        [InlineData("sixteen")]
        [InlineData("seventeen")]
        [InlineData("eighteen")]
        [InlineData("nineteen")]
        [InlineData("twenty")]
        public async Task Create_Container_Should_Succeed(string text)
        {
            var documentId = Guid.NewGuid().ToString();
            var createdOn = DateTime.UtcNow;
            var document = new CosmosDbDocument(documentId, text, createdOn);
            var response = await HttpClient.PostAsJsonAsync("runsample", document);
            var commandHttpResponse = await response.ToPayload<CommandHttpResponse>();

            response.Should().BeSuccessful();
            commandHttpResponse.Payload.Text.Should().Be(document.text);
        }
    }
    
    public class GivenDatabaseAvailableAndGivenWhenThenApproach(TestContainerFixture fixture, ITestOutputHelper output) 
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
            commandHttpResponse.Payload.Text.Should().Be(_document.text);
        }
    }
}