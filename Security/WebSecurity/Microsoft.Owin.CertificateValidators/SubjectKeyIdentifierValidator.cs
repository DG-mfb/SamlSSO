﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel.Security.Validation;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators
{
    internal class SubjectKeyIdentifierValidator : CertificateSubjectKeyIdentifierValidator, IPinningSertificateValidator
    {
        public SubjectKeyIdentifierValidator(IEnumerable<string> validSubjectKeyIdentifiers)
            : base(validSubjectKeyIdentifiers)
        {
        }

        public Task Validate(object sender, BackchannelCertificateValidationContext context, Func<object, BackchannelCertificateValidationContext, Task> next)
        {
            throw new NotImplementedException();
        }
    }
}