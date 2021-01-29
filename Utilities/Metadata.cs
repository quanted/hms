using System.Collections.Generic;

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

        public static Dictionary<string, string> MergeMetadata(Dictionary<string, string> dict1, Dictionary<string, string> dict2, string prefix=null)
        {
            foreach(KeyValuePair<string, string> kv in dict2)
            {
                string key = (prefix != null) ? prefix + "_" + kv.Key : kv.Key;
                if (!dict1.ContainsKey(key))
                {
                    dict1.Add(key, kv.Value);
                }
            }
            return dict1;
        }

        public static Dictionary<string, string> MergeMetadata(Dictionary<string, string> dict1, Dictionary<string, object> dict2, string prefix=null)
        {
            foreach (KeyValuePair<string, object> kv in dict2)
            {
                string key = (prefix != null) ? prefix + "_" + kv.Key : kv.Key;
                if (!dict1.ContainsKey(key))
                {
                    dict1.Add(key, kv.Value.ToString());
                }
            }
            return dict1;
        }

    }
}
