﻿using System.Collections.Generic;

namespace Utilities
{
    public class Metadata
    {

        public static Dictionary<string, string> AddToMetadata(string key, string value, Dictionary<string, string> metadata)
        {
            if (!metadata.ContainsKey(key))
            {
                metadata.Add(key, value);
            }
            return metadata;
        }
    }
}
