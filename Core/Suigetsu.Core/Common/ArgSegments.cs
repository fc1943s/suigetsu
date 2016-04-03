using System.Collections.Generic;
using System.Linq;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Common
{
    public static class ArgSegments
    {
        public static Dictionary<string, string> Parse(IEnumerable<string> args, bool usePrefix = true)
        {
            var result = new Dictionary<string, string>();

            string[] current = { string.Empty, string.Empty };

            if(usePrefix)
            {
                foreach(var v in args.Where(v => !v.IsEmpty()))
                {
                    if(v[0].In('-', '/'))
                    {
                        if(current[0] != string.Empty)
                        {
                            result.Add(current[0], current[1].Trim());
                        }

                        current[0] = v.Substring(1).Trim();
                        current[1] = string.Empty;
                    }
                    else
                    {
                        if(current[0] != string.Empty)
                        {
                            current[1] += " " + v;
                        }
                    }
                }
            }
            else
            {
                foreach(var v in args)
                {
                    if(current[0] == string.Empty)
                    {
                        current[0] = v.Trim();
                    }
                    else
                    {
                        result.Add(current[0], v.Trim());
                        current[0] = string.Empty;
                    }
                }
            }

            if(current[0] != string.Empty)
            {
                result.Add(current[0], current[1].Trim());
            }

            return result;
        }
    }
}
