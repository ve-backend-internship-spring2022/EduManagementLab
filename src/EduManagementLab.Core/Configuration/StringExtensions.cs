using System.Diagnostics;
using IdentityServer4.Extensions;

namespace EduManagementLab.Core.Configuration
{
    /// <summary>
    /// Local version of Identity Server 4 internal static class StringExtensions
    /// https://github.com/IdentityServer/IdentityServer4/blob/master/src/Extensions/StringsExtensions.cs
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Ensure the string has a trailing slash.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }
        
        /// <summary>
        /// True if the value is null, empty, or whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// True if the value is not null, empty, or whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool TryConvertToDictionary(this string properties, out Dictionary<string, string> dictionary)
        {
            dictionary = default(Dictionary<string, string>);
            if (string.IsNullOrWhiteSpace(properties))
            {
                return false;
            }

            dictionary = new Dictionary<string, string>();

            var lines = properties.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var pair = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length != 2)
                {
                    dictionary = null;
                    return false;
                }

                var key = pair[0];
                if (string.IsNullOrWhiteSpace(key))
                {
                    dictionary = null;
                    return false;
                }

                var value = pair[1];
                if (string.IsNullOrWhiteSpace(value))
                {
                    dictionary = null;
                    return false;
                }

                if (!dictionary.TryAdd(key, value))
                {
                    dictionary = null;
                    return false;
                }
            }

            return true;
        }
    }
}
