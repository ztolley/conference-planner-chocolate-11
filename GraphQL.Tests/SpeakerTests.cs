using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Execution;
using Snapshooter.Xunit;
using Xunit;

namespace GraphQL.Tests
{
    public class SpeakerTests
    {
        [Fact]
        public async Task Speaker_Schema_Changed()
        {
            // arrange
            // act
            ISchema schema = await new ServiceCollection()
                .AddPooledDbContextFactory<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("Data Source=conferences.db"))
                .AddGraphQL()
                .AddQueryType(d => d.Name("Query"))
                .AddType<SpeakerQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<SpeakerMutations>()
                .AddType<SessionType>()
                .AddType<SpeakerType>()
                .AddType<TrackType>()
                .EnableRelaySupport()
                .BuildSchemaAsync();
             
            // assert
            schema.Print().MatchSnapshot();
        }

        [Fact]
        public async Task AddSpeaker()
        {
            // arrange
            IServiceProvider services = new ServiceCollection()
                .AddPooledDbContextFactory<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("Data Source=conferences.db"))
                .AddGraphQL()
                .AddQueryType(d => d.Name("Query"))
                    .AddType<SpeakerQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddType<SpeakerMutations>()
                .AddType<SessionType>()
                .AddType<SpeakerType>()
                .AddType<TrackType>()
                .EnableRelaySupport()
                .Services
                .BuildServiceProvider();

            // act
            IExecutionResult result = await services.ExecuteRequestAsync(
                QueryRequestBuilder.New()
                    .SetQuery(@"
                        mutation AddSpeaker {
                            addSpeaker(input: {
                                name: ""Dolly Smith""
                                bio: ""Folly is really cool""
                            webSite: ""http://dolly.com""
                        }) {
                            speaker {
                                id
                                    name
                                bio
                                    webSite
                            }
                        }
                    }")
                    .Create());

            // assert
            result.ToJson().MatchSnapshot();
        }
    }
}