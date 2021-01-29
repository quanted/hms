using System.Collections.Generic;

namespace Utilities
{
    public class Converter
    {

        public static Dictionary<string, string> ConvertDict(Dictionary<string, object> dict)
        {
            Dictionary<string, string> convertedDict = new Dictionary<string, string>();
            foreach(KeyValuePair<string, object> kv in dict)
            {
                convertedDict.Add(kv.Key, kv.Value.ToString());
            }
            return convertedDict;
        }

        public static Dictionary<string, object> ConvertDict(Dictionary<string, string> dict)
        {
            Dictionary<string, object> convertedDict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> kv in dict)
            {
                convertedDict.Add(kv.Key, (object)kv.Value);
            }
            return convertedDict;
        }

    }
}
