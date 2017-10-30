using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.MetaData;
using Kernel.Initialisation;
using Kernel.Logging;
using ServerInitialisation;
using UnityResolver;

namespace WsFederationMetadataTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\Dan\Software\Apira\SPMetadata\SPMetadataTest0123.xml";
            using (new InformationLogEventWriter())
            {
                ApplicationConfiguration.RegisterDependencyResolver(() => new UnityDependencyResolver());
                ApplicationConfiguration.RegisterServerInitialiserFactory(() => new ServerInitialiser());
                var task = Initialisation.Init();
                task.Wait();
            }

            var metadataTask = Program.CreateMetadata(path);
            metadataTask.Wait();
            Console.ReadLine();
        }

        static async Task CreateMetadata(string path)
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                var federationParty = "local";//FederationPartyIdentifierHelper.GetFederationPartyIdFromRequestOrDefault(c);
                var metadataGenerator = Program.ResolveMetadataGenerator<ISPMetadataGenerator>();

                var metadataRequest = new MetadataGenerateRequest(MetadataType.SP, federationParty, new MetadataPublishContext(fs, MetadataPublishProtocol.FileSystem));
                await metadataGenerator.CreateMetadata(metadataRequest);
            }
        }

        private static TMetadatGenerator ResolveMetadataGenerator<TMetadatGenerator>() where TMetadatGenerator : IMetadataGenerator
        {
            var resolver = ApplicationConfiguration.Instance.DependencyResolver;
            var metadataGenerator = resolver.Resolve<TMetadatGenerator>();
            return metadataGenerator;
        }
    }
}