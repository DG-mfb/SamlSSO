﻿using System;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Response;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Constants;
using NUnit.Framework;

namespace Federation.Protocols.Test.Response
{
    [TestFixture]
    internal class SamlTokenResponseParserTests
    {
        [Test]
        public async Task ParseResponse_success_Test()
        {
            //ARRANGE
            var inResponseTo = "Test_" + Guid.NewGuid().ToString();
            
            var response = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo, StatusCodes.Success);
            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);
            
            var parser = new SamlTokenResponseParser(logger);
            //ACT
            var tokenResponse = await parser.Parse(serialised);
            //ASSERT
            Assert.AreEqual(StatusCodes.Success, tokenResponse.Status.StatusCode.Value);
            Assert.IsNull(tokenResponse.Status.StatusCode.SubStatusCode);
            Assert.IsNull(tokenResponse.Status.StatusMessage);
            Assert.IsNull(tokenResponse.Status.StatusDetail);
            Assert.AreEqual(inResponseTo, tokenResponse.InResponseTo);
            Assert.AreEqual(response.ID, tokenResponse.ID);
            Assert.AreEqual(response.IssueInstant, tokenResponse.IssueInstant);
            Assert.AreEqual(response.Issuer.Format, tokenResponse.Issuer.Format);
            Assert.AreEqual(response.Issuer.Value, tokenResponse.Issuer.Value);
            Assert.AreEqual(response.Version, tokenResponse.Version);
            Assert.AreEqual(response.Destination, tokenResponse.Destination);
        }

        [Test]
        public async Task ParseResponse_failed_one_status_code_Test()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();

            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, StatusCodes.Responder);
            response.Status.StatusMessage = "Test message";
            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);

            var parser = new SamlTokenResponseParser(logger);
            //ACT
            var tokenResponse = await parser.Parse(serialised);
            //ASSERT
            Assert.AreEqual(StatusCodes.Responder, tokenResponse.Status.StatusCode.Value);
            Assert.IsNull(tokenResponse.Status.StatusCode.SubStatusCode);
            Assert.AreEqual("Test message", tokenResponse.Status.StatusMessage);
            Assert.IsNull(tokenResponse.Status.StatusDetail);
        }

        [Test]
        public async Task ParseResponse_failed_nested_status_code_Test()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();

            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, StatusCodes.Responder);
            ResponseFactoryMock.GetStatusCode(StatusCodes.NoAuthnContext, response.Status.StatusCode);
            response.Status.StatusMessage = "Test message";

            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);

            var parser = new SamlTokenResponseParser(logger);
            //ACT
            var tokenResponse = await parser.Parse(serialised);
            //ASSERT
            Assert.AreEqual(StatusCodes.Responder, tokenResponse.Status.StatusCode.Value);
            Assert.IsNotNull(tokenResponse.Status.StatusCode.SubStatusCode);
            Assert.AreEqual(StatusCodes.NoAuthnContext, tokenResponse.Status.StatusCode.SubStatusCode.Value);
            Assert.IsNull(tokenResponse.Status.StatusCode.SubStatusCode.SubStatusCode);
            Assert.AreEqual("Test message", tokenResponse.Status.StatusMessage);
            Assert.IsNull(tokenResponse.Status.StatusDetail);
        }

        [Test]
        public async Task ParseResponse_failed_2_nested_status_code_Test()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();

            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, StatusCodes.Responder);
            ResponseFactoryMock.GetStatusCode(StatusCodes.NoAuthnContext, response.Status.StatusCode);
            ResponseFactoryMock.GetStatusCode(StatusCodes.InvalidNameIdPolicy, response.Status.StatusCode.SubStatusCode);
            response.Status.StatusMessage = "Test message";

            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);

            var parser = new SamlTokenResponseParser(logger);
            //ACT
            var tokenResponse = await parser.Parse(serialised);
            //ASSERT
            Assert.AreEqual(StatusCodes.Responder, tokenResponse.Status.StatusCode.Value);
            Assert.IsNotNull(tokenResponse.Status.StatusCode.SubStatusCode);
            Assert.AreEqual(StatusCodes.NoAuthnContext, tokenResponse.Status.StatusCode.SubStatusCode.Value);
            Assert.IsNotNull(tokenResponse.Status.StatusCode.SubStatusCode.SubStatusCode);
            Assert.AreEqual(StatusCodes.InvalidNameIdPolicy, tokenResponse.Status.StatusCode.SubStatusCode.SubStatusCode.Value);
            Assert.IsNull(tokenResponse.Status.StatusCode.SubStatusCode.SubStatusCode.SubStatusCode);
            Assert.AreEqual("Test message", tokenResponse.Status.StatusMessage);
            Assert.IsNull(tokenResponse.Status.StatusDetail);
        }

        [Test]
        public async Task ParseResponse_failed_nested_status_code_with_details_Test()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();

            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, StatusCodes.Responder);
            ResponseFactoryMock.GetStatusCode(StatusCodes.NoAuthnContext, response.Status.StatusCode);
            response.Status.StatusMessage = "Test message";
            var doc = new XmlDocument();
            //doc.PreserveWhitespace = true;
            
            var el = doc.CreateElement("DetailElement");
            el.SetAttribute("MessageId", "Status details");
            var el1 = doc.CreateElement("AditionalDetailElement");
            el1.SetAttribute("AdditionalDetail", "Additional details");
            ResponseFactoryMock.BuildStatuseDetail(response.Status, new[] { el, el1 });
            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);

            var parser = new SamlTokenResponseParser(logger);
            //ACT
            var tokenResponse = await parser.Parse(serialised);
            //ASSERT
            Assert.AreEqual(StatusCodes.Responder, tokenResponse.Status.StatusCode.Value);
            Assert.IsNotNull(tokenResponse.Status.StatusCode.SubStatusCode);
            Assert.AreEqual(StatusCodes.NoAuthnContext, tokenResponse.Status.StatusCode.SubStatusCode.Value);
            Assert.IsNull(tokenResponse.Status.StatusCode.SubStatusCode.SubStatusCode);
            Assert.AreEqual("Test message", tokenResponse.Status.StatusMessage);
            Assert.IsNotNull(tokenResponse.Status.StatusDetail);
            Assert.AreEqual(2, tokenResponse.Status.StatusDetail.Any.Length);
        }
    }
}