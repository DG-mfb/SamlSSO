using System;
using System.IO;
using System.Net;
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
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Verbose) Console.WriteLine("Filename: {0}", options.MetadataFilePath);
            }
            else
            {
                var usage = options.GetUsage();
                Console.WriteLine(usage);
                Console.ReadLine();
                return;
            }

            using (new InformationLogEventWriter())
            {
                ApplicationConfiguration.RegisterDependencyResolver(() => new UnityDependencyResolver());
                ApplicationConfiguration.RegisterServerInitialiserFactory(() => new ServerInitialiser());
                var task = Initialisation.Init();
                task.Wait();
            }

            var metadataTask = Program.CreateMetadata(options);
            metadataTask.Wait();
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        static async Task CreateMetadata(Options options)
        {
            if (File.Exists(options.MetadataFilePath))
            {
                if (options.ReplaceFile)
                    File.Delete(options.MetadataFilePath);
                else
                {
                    Console.WriteLine(String.Format("File with the name: {0} exists in path specified:\r\n {1}. To replace the file use options -r.\r\nCreating metadata result: FAILED.", Path.GetFileName(options.MetadataFilePath), Path.GetDirectoryName(options.MetadataFilePath)));
                    return;
                }
            }

            var request = WebRequest.Create(String.Format("file://{0}", options.MetadataFilePath));
            request.Method = "POST";
            using (var fs = request.GetRequestStream())
            {
                var federationParty = options.FederationPartyId;
                var metadataGenerator = Program.ResolveMetadataGenerator<ISPMetadataGenerator>();

                var metadataRequest = new MetadataGenerateRequest(MetadataType.SP, federationParty, new MetadataPublicationContext(fs, MetadataPublicationProtocol.FileSystem));
                await metadataGenerator.CreateMetadata(metadataRequest);
                Console.WriteLine("Create metadata file result: SUCCESS.");
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