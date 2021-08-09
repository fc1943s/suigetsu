using System;
using NUnit.Framework;
using Suigetsu.Core.IO;

namespace Suigetsu.Core.Tests.IO
{
    [TestFixture]
    public class TempFileTests : AssertionHelper
    {
        [Test]
        public void TempFileTest()
        {
            Expect(() => new TempFile(""), Throws.InstanceOf<ArgumentNullException>());

            var file = new TempFile();

            file.Dispose();

            file.CloseHandle();

            GC.WaitForPendingFinalizers();

            Expect(() => file.FilePath, Throws.InstanceOf<ObjectDisposedException>());
        }
    }
}
