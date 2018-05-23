using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using Kernel.Federation.MetaData.Configuration.Organisation;

namespace WsFederationMetadataProvider.Metadata.DescriptorBuilders
{
    internal class SSODescriptorBuilderHelper
    {
        internal static bool TryBuildOrganisation(OrganisationConfiguration organisationConfiguration, out Organization organisation)
        {
            organisation = new Organization();
            if (organisationConfiguration == null)
                return false;
            try
            {
                organisationConfiguration.Names.Aggregate(organisation, (o, next) =>
                {
                    o.Names.Add(new LocalizedName(next.Name, next.Language));
                    o.DisplayNames.Add(new LocalizedName(next.DisplayName, next.Language));
                    return o;
                });
                organisationConfiguration.Urls.Aggregate(organisation, (o, next) =>
                {
                    o.Urls.Add(new LocalizedUri(next.Url, next.Language));
                    return o;
                });

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        internal static void BuildContacts(ICollection<System.IdentityModel.Metadata.ContactPerson> contacts, OrganisationConfiguration organisationConfiguration)
        {
            if (contacts == null)
                throw new ArgumentNullException("contacts");
            if (organisationConfiguration == null || organisationConfiguration.OrganisationContacts == null || organisationConfiguration.OrganisationContacts.PersonContact == null)
                return;

            organisationConfiguration.OrganisationContacts.PersonContact.Aggregate(contacts, (c, next) =>
            {
                System.IdentityModel.Metadata.ContactType contactType;
                if (!Enum.TryParse<System.IdentityModel.Metadata.ContactType>(next.ContactType.ToString(), out contactType))
                    throw new InvalidCastException(String.Format("No corespondenting value for Contact type: {0}.", next.ContactType));
                var cp = new System.IdentityModel.Metadata.ContactPerson(contactType)
                {
                    Surname = next.SurName,
                    GivenName = next.ForeName,
                };
                next.Emails.Aggregate(cp.EmailAddresses, (p, nextEmail) => {p.Add(nextEmail); return p; });
                next.PhoneNumbers.Aggregate(cp.TelephoneNumbers, (p, nextNumber) => { p.Add(nextNumber); return p; });
                c.Add(cp);
                return c;
            });
        }
    }
}