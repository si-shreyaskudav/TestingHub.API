using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingHub.API.Core;
using Xunit;

namespace TestingHub.API
{
    public class SampleTest : TestFramework<Program>
    {
        public SampleTest()
        {
            //base.GenerateTestCase();
        }

        [Fact]
        public async void SampleTest1()
        {
            // Arrange
            var testCase = testCases["<testCase_key_here>"];

            // Act  
            var response = await client.CallAsync(testCase);

            // Assert
            response.SuccessAsserts();

            // Custom asserts
            string gamedayid = response.ExtractFromData<string>("value.gamedayid");
            gamedayid.Should().NotBeNullOrEmpty();
            int matchId = response.ExtractFromData<int>("value.matchid");
            matchId.Should().NotBe(0);

            // For more details on writing assert conditions, FluentAssertion refer documentation
        }
    }
}
