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
        public IEnumerable<TValidator> Resolve<TValidator>(Uri partnerId) where TValidator : class
        {
            if (!typeof(IPinningSertificateValidator).IsAssignableFrom(typeof(TValidator)))
                return Enumerable.Empty<TValidator>();

            if (partnerId == null)
                throw new ArgumentNullException("partnerId");
            var configuration = this._configurationProvider.GeBackchannelConfiguration(partnerId);
            return this.Resolve(configuration).Cast<TValidator>();
        }

        public IEnumerable<IPinningSertificateValidator> Resolve(BackchannelConfiguration configuration)
        {
            var pins = configuration.Pins;
            if (pins == null || pins.Count == 0)
                return Enumerable.Empty<IPinningSertificateValidator>();

            var validThumbprints = pins.ContainsKey(PinType.Thumbprint) ? pins[PinType.Thumbprint] : Enumerable.Empty<string>();
            var thumbprintValidator = new ThumbprintValidator(validThumbprints);

            var validSubjectKeyIdentifiers = pins.ContainsKey(PinType.SubjectKeyIdentifier) ? pins[PinType.SubjectKeyIdentifier] : Enumerable.Empty<string>();
            var subjectKeyIdentifierValidator = validSubjectKeyIdentifiers.Count() == 0 ? (IPinningSertificateValidator)null : new SubjectKeyIdentifierValidator(validSubjectKeyIdentifiers);

            var validBase64EncodedSubjectPublicKeyInfoHashes = pins.ContainsKey(PinType.SubjectPublicKeyInfo) ? pins[PinType.SubjectPublicKeyInfo] : Enumerable.Empty<string>();
            var subjectPublicKeyInfoValidator = validBase64EncodedSubjectPublicKeyInfoHashes.Count() == 0 ? (IPinningSertificateValidator)null : new SubjectPublicKeyInfoValidator(validSubjectKeyIdentifiers, Security.SubjectPublicKeyInfoAlgorithm.Sha256);

            return new[] { thumbprintValidator, subjectKeyIdentifierValidator, subjectPublicKeyInfoValidator };
        }
    }
}