using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Federation.Protocols.Response;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using Serialisation.Xml;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Test.Response
{
    [TestFixture]
    internal class SamlTokenResponseParserTests
    {
        [Test]
        public async Task ParseResponseTest()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();
            
            var response = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo, StatusCodes.Success);
            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);
            //var toBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(serialised));
            //var context = new SamlInboundContext
            //{
            //    Form = new Dictionary<string, string> { { HttpRedirectBindingConstants.SamlResponse, toBase64 } }
            //};
            
            var parser = new SamlTokenResponseParser(logger);
            //ACT
            var tokenResponse = await parser.ParseResponse(serialised);
            //ASSERT

        }
    }
}
