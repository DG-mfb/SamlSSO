using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ORMMetadataContextProvider.FederationParty;
using ORMMetadataContextProvider.Tests.Mock;

namespace ORMMetadataContextProvider.Tests
{
    [TestFixture]
    internal class FederationPartyContextBuilderTests
    {
        [Test]
        public void PutCacheTest()
        {
            //ARRANGE
            var context = new DbContextMock();
            context.Initialise();
            var cache = new CacheProviderMock();
            var federationPartyContextBuilder = new FederationPartyContextBuilder(context, cache);
            //ACT
            var before = cache.Contains("local");
            var federationContext = federationPartyContextBuilder.BuildContext("local");
            var after = cache.Contains("local");
            //ASSERT
            Assert.IsFalse(before);
            Assert.IsTrue(after);
        }

        [Test]
        public void ReadFromCacheTest()
        {
            //ARRANGE
            var readFromCache = false;
            var context = new DbContextMock();
            context.Initialise();
            var cache = new CacheProviderMock();
            cache.ReadFrom += (_, __) => { readFromCache = true; };
            var federationPartyContextBuilder = new FederationPartyContextBuilder(context, cache);
            //ACT
            var before = cache.Contains("local");
            var federationContext = federationPartyContextBuilder.BuildContext("local");
            var after = cache.Contains("local");
            var federationContextFromCache = federationPartyContextBuilder.BuildContext("local");
            //ASSERT
            Assert.IsFalse(before);
            Assert.IsTrue(after);
            Assert.IsTrue(readFromCache);
            Assert.AreSame(federationContext, federationContextFromCache);
        }

        [Test]
        public void RemoveFromCacheTest()
        {
            //ARRANGE
            var readFromCache = false;
            var context = new DbContextMock();
            context.Initialise();
            var cache = new CacheProviderMock();
            cache.ReadFrom += (_, __) => { readFromCache = true; };
            var federationPartyContextBuilder = new FederationPartyContextBuilder(context, cache);
            //ACT
            var before = cache.Contains("local");
            var federationContext = federationPartyContextBuilder.BuildContext("local");
            var after = cache.Contains("local");
            var federationContextFromCache = federationPartyContextBuilder.BuildContext("local");
            federationPartyContextBuilder.RequestRefresh("local");
            var afterRemove = cache.Contains("local");
            //ASSERT
            Assert.IsFalse(before);
            Assert.IsTrue(after);
            Assert.IsTrue(readFromCache);
            Assert.AreSame(federationContext, federationContextFromCache);
            Assert.IsFalse(afterRemove);
        }
    }
}