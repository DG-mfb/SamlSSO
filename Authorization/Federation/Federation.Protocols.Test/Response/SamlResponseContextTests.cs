using System;
using Federation.Protocols.Test.Mock;
using NUnit.Framework;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Test.Response
{
    [TestFixture]
    internal class SamlResponseContextTests
    {
        [Test]
        public void ResponseStatusTest_success_response()
        {
            //ARRANGE

            var inResponseTo = Guid.NewGuid().ToString();
            var expectedAggregatedMessage = String.Format("StatusCode: {0}\r\n", StatusCodes.Success);
            var responseStatus = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo, StatusCodes.Success);
            var responseContext = new SamlResponseContext { StatusResponse = responseStatus };
            //ACT

            //ASSERT
            Assert.IsTrue(responseContext.IsSuccess);
            Assert.AreEqual(StatusCodes.Success, responseContext.StatusResponse.Status.StatusCode.Value);
            Assert.IsFalse(responseContext.IsIdpInitiated);
            Assert.AreEqual(expectedAggregatedMessage, responseContext.AggregatedMessage);
        }

        [Test]
        public void ResponseStatusTest_success_response_with_additional_status_code()
        {

            ////ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();
            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, StatusCodes.Responder);
            ResponseFactoryMock.GetStatusCode(StatusCodes.NoAuthnContext, response.Status.StatusCode);
            var expectedAggregatedMessage = String.Format("StatusCode: {0}\r\nAdditional status code: {1}\r\n", StatusCodes.Responder, StatusCodes.NoAuthnContext);
            var responseContext = new SamlResponseContext { StatusResponse = response };

            ////ACT

            ////ASSERT
            Assert.IsFalse(responseContext.IsSuccess);
            Assert.AreEqual(StatusCodes.Responder, responseContext.StatusResponse.Status.StatusCode.Value);
            Assert.AreEqual(StatusCodes.NoAuthnContext, responseContext.StatusResponse.Status.StatusCode.SubStatusCode.Value);
            Assert.AreEqual(expectedAggregatedMessage, responseContext.AggregatedMessage);
        }

        [Test]
        public void ResponseStatusTest_fail_response_with_additional_status_code_and_message()
        {
            ////ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();
            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, StatusCodes.Responder);
            ResponseFactoryMock.GetStatusCode(StatusCodes.NoAuthnContext, response.Status.StatusCode);
            response.Status.StatusMessage = "Test message";
            var expectedAggregatedMessage = String.Format("StatusCode: {0}\r\nAdditional status code: {1}\r\nMessage status: {2}\r\n", StatusCodes.Responder, StatusCodes.NoAuthnContext, "Test message");
            var responseContext = new SamlResponseContext { StatusResponse = response };

            ////ACT

            ////ASSERT
            Assert.IsFalse(responseContext.IsSuccess);
            Assert.AreEqual(StatusCodes.Responder, responseContext.StatusResponse.Status.StatusCode.Value);
            Assert.AreEqual(StatusCodes.NoAuthnContext, responseContext.StatusResponse.Status.StatusCode.SubStatusCode.Value);
            Assert.AreEqual(expectedAggregatedMessage, responseContext.AggregatedMessage);
        }

        [Test]
        public void ResponseStatusTest_fail_response_with_additional_status_code_and_message_details()
        {
            throw new NotImplementedException();
            ////ARRANGE
            //var expectedStatusMessage = "An error occured.";
            //var expectedMessageDetails = "Some details";
            //var expectedAggregatedMessage = String.Format("StatusCode: {0}\r\nAdditional status code: {1}\r\nMessage status: {2}\r\nMessage details: {3}", StatusCodes.Requester, StatusCodes.InvalidNameIdPolicy, expectedStatusMessage, expectedMessageDetails);

            //var responseStatus = new SamlResponseContext();
            //responseStatus.StatusCodes.Add(StatusCodes.Requester);
            //responseStatus.StatusCodes.Add(StatusCodes.InvalidNameIdPolicy);
            //responseStatus.StatusMessage = "An error occured.";
            //responseStatus.MessageDetails = expectedMessageDetails;
            ////ACT

            ////ASSERT
            //Assert.IsFalse(responseStatus.IsSuccess);
            //Assert.AreEqual(StatusCodes.Requester, responseStatus.StatusCodeMain);
            //Assert.AreEqual(StatusCodes.InvalidNameIdPolicy, responseStatus.AdittionalStatusCode);
            //Assert.AreEqual(expectedStatusMessage, responseStatus.StatusMessage);
            //Assert.AreEqual(expectedMessageDetails, responseStatus.MessageDetails);
            //Assert.AreEqual(expectedAggregatedMessage, responseStatus.AggregatedMessage);
        }
    }
}