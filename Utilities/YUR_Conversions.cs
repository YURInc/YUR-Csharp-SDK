using UnityEngine;

namespace YUR.SDK.Unity.Utilities
{
    class YUR_Conversions
    {
        /// <summary>
        /// Convert DATA to a JSON Ready Class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DATA"></param>
        /// <param name="class"></param>
        public static void ConvertStringToObjectOverwrite<T>(string DATA, ref T @class)
        {
            JsonUtility.FromJsonOverwrite(DATA, @class);
        }

        public static T ConvertStringToObject<T>(string DATA)
        {
           return JsonUtility.FromJson<T>(DATA);
        }

        public static void ConvertObjectToString<T>(T @object, out string JSON_string)
        {
            JSON_string = JsonUtility.ToJson(@object);
        }

        public static string ConvertObjectToString<T>(T @object)
        {
            return JsonUtility.ToJson(@object);
        }

        internal static string PathCombine(string path1, string path2)
        {
            if (System.IO.Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(System.IO.Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(System.IO.Path.AltDirectorySeparatorChar);
            }

            return System.IO.Path.Combine(path1, path2);
        }

    }
}
