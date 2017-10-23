using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;

namespace Microsoft.Owin.CertificateValidators
{
    internal class CertificateValidatorResolver : ICertificateValidatorResolver
    {
        ICertificateValidationConfigurationProvider _configurationProvider;
        public CertificateValidatorResolver(ICertificateValidationConfigurationProvider configurationProvider)
        {
            this._configurationProvider = configurationProvider;
        }
        public IEnumerable<TValidator> Resolve<TValidator>(string partnerId) where TValidator : class
        {
            if (!typeof(IPinningSertificateValidator).IsAssignableFrom(typeof(TValidator)))
                return Enumerable.Empty<TValidator>();

            if (String.IsNullOrWhiteSpace(partnerId))
                throw new ArgumentNullException("partnerId");
            var configuration = this._configurationProvider.GeBackchannelConfiguration(partnerId);
            var pins = configuration.Pins;
            if(pins == null || pins.Count == 0)
                return Enumerable.Empty<TValidator>();

            var validThumbprints = pins.ContainsKey(PinType.Thumbprint) ? pins[PinType.Thumbprint] : Enumerable.Empty<string>();
            var thumbprintValidator = new ThumbprintValidator(validThumbprints) as TValidator;

            var validSubjectKeyIdentifiers = pins.ContainsKey(PinType.SubjectKeyIdentifier) ? pins[PinType.SubjectKeyIdentifier] : Enumerable.Empty<string>();
            var subjectKeyIdentifierValidator = validSubjectKeyIdentifiers.Count() == 0 ? default(TValidator) : new SubjectKeyIdentifierValidator(validSubjectKeyIdentifiers) as TValidator;

            var validBase64EncodedSubjectPublicKeyInfoHashes = pins.ContainsKey(PinType.SubjectPublicKeyInfo) ? pins[PinType.SubjectPublicKeyInfo] : Enumerable.Empty<string>();
            var subjectPublicKeyInfoValidator = validBase64EncodedSubjectPublicKeyInfoHashes.Count() == 0 ? default(TValidator) : new SubjectPublicKeyInfoValidator(validSubjectKeyIdentifiers, Security.SubjectPublicKeyInfoAlgorithm.Sha256) as TValidator;

            return new [] { thumbprintValidator, subjectKeyIdentifierValidator, subjectPublicKeyInfoValidator };
        }
    }
}