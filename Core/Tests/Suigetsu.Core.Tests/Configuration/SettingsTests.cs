using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Suigetsu.Core.Configuration;

namespace Suigetsu.Core.Tests.Configuration
{
    [TestFixture]
    public class SettingsTests : AssertionHelper
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class TestSettings : Settings
        {
            public string EmptySetting { get; protected set; }
        }

        [Test]
        public void SettingsTest()
        {
            Expect(() => Settings.Get<TestSettings>().EmptySetting, Throws.InstanceOf<Exception>()); //TODO: exception type
        }
    }
}
