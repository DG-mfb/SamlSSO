using System.Globalization;
using Kernel.Data.ORM;
using ORMMetadataContextProvider.Models;

namespace ORMMetadataContextProvider.Seeders
{
    internal class OrganisationSeeder : Seeder
    {
        public override void Seed(IDbContext context)
        {
            var organisation = new OrganisationSettings();
            organisation.Names.Add(new LocalisedName { Language = CultureInfo.CurrentCulture.Name, Name = "Company name." });
            organisation.Urls.Add(new LocalisedName { Language = CultureInfo.CurrentCulture.Name, Name = "https://company.co.uk/" });
            var contact = new PersonContact
            {
                ContactType = Kernel.Federation.MetaData.Configuration.Organisation.ContactType.Technical,
                Forename = new LocalisedName { Name = "John", Language = CultureInfo.CurrentCulture.Name },
                Surname = new LocalisedName { Name = "Doe", Language = CultureInfo.CurrentCulture.Name },
            };
            contact.Emails.Add(new LocalisedName { Language = CultureInfo.CurrentCulture.Name, Name = "john.doe@comain.co.uk" });
            contact.Phones.Add(new Phone { Type = PhoneType.Working, Number = "020 123 456" });
            contact.Organisations.Add(organisation);
            organisation.Contacts.Add(contact);
            Seeder._cache.Add(Seeder.Organisation, organisation);
            context.Add<OrganisationSettings>(organisation);
        }
    }
}