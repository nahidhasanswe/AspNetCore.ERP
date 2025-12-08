using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ERP.Core.Extensions
{
    public static class StringExtensions
    {
        static readonly Regex re = new Regex(@"\{([^\}]+)\}", RegexOptions.Compiled);
        public static string ReplaceMatch(this string text, StringDictionary fields)
        {
            return re.Replace(text, match => fields[match.Groups[1].Value]);
        }

        public static bool IsBase64String(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
                || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                // Handle the exception
            }
            return false;
        }

        public static Stream ToMemoryStream(this string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            return new MemoryStream(bytes, 0, bytes.Length);
        }
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
        
        public static string ToCamelCase(this string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                });
            var tailWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                .ToArray();
            return $"{leadWord}{string.Join(string.Empty, tailWords)}";
        }

        public static int ToConvertInt(this string? str)
        {
            if (int.TryParse(str, out var result))
                return result;
            
            return -1;
        }

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }

        public static string ToSEOName(this string name)
        {
            return name.Replace(" ", "_") + $"_{Guid.NewGuid().ToString().Replace("-", "_")}";
        }
    }
}
