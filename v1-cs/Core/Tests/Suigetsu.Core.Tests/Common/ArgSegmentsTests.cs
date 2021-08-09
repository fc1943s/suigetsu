using System.Collections.Generic;
using NUnit.Framework;
using Suigetsu.Core.Common;

namespace Suigetsu.Core.Tests.Common
{
    [TestFixture]
    public class ArgSegmentsTests : AssertionHelper
    {
        [Test]
        public void ParseTest()
        {
            var args = new[] { "-s", "/f", "-t", "01", string.Empty, "/xx", string.Empty, "A", "D", "-path", "C:/New", "Folder" };

            Expect
                (ArgSegments.Parse(args),
                 EquivalentTo
                     (new Dictionary<string, string>
                      {
                          { "s", string.Empty },
                          { "f", string.Empty },
                          { "t", "01" },
                          { "xx", "A D" },
                          { "path", "C:/New Folder" }
                      }));

            Expect
                (ArgSegments.Parse(args, false),
                 EquivalentTo
                     (new Dictionary<string, string>
                      {
                          { "-s", "/f" },
                          { "-t", "01" },
                          { "/xx", string.Empty },
                          { "A", "D" },
                          { "-path", "C:/New" },
                          { "Folder", string.Empty }
                      }));
        }
    }
}
