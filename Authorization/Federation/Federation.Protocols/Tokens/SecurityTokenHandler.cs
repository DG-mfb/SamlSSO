﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Kernel.Authentication.Claims;
using Kernel.Federation.Tokens;

namespace Federation.Protocols.Tokens
{
    internal class SecurityTokenHandler : ITokenHandler
    {
        private readonly ITokenSerialiser _tokenSerialiser;
        private readonly ITokenValidator _tokenValidator;
        private readonly IUserClaimsProvider<Tuple<Saml2SecurityToken, HandleTokenContext>> _identityProvider;

        public SecurityTokenHandler(ITokenSerialiser tokenSerialiser, ITokenValidator tokenValidator, IUserClaimsProvider<Tuple<Saml2SecurityToken, HandleTokenContext>> identityProvider)
        {
            this._tokenSerialiser = tokenSerialiser;
            this._tokenValidator = tokenValidator;
            this._identityProvider = identityProvider;
        }
        
        public async Task<TokenHandlingResponse> HandleToken(HandleTokenContext context)
        {
            var partnerId = context._federationPartyId;
            ClaimsIdentity identity = null;
            var token = this._tokenSerialiser.DeserialiseToken(context.Token, partnerId);
            var validationResult = new List<ValidationResult>();
            var isValid = this._tokenValidator.Validate(token, validationResult, partnerId);

            if (isValid)
            {
                var identities = await this._identityProvider.GenerateUserIdentitiesAsync(new Tuple<Saml2SecurityToken, HandleTokenContext>((Saml2SecurityToken)token, context), new[] { context.AuthenticationMethod });
                identity = identities[context.AuthenticationMethod];
            }

            return new TokenHandlingResponse(token, identity, validationResult);
        }
    }
}