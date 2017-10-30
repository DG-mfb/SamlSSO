using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            using (new InformationLogEventWriter())
            {
                ApplicationConfiguration.RegisterDependencyResolver(() => new UnityDependencyResolver());
                ApplicationConfiguration.RegisterServerInitialiserFactory(() => new ServerInitialiser());
                var task = Initialisation.Init();
                task.Wait();
            }
        }
    }
}