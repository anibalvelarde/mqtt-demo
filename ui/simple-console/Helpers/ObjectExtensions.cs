using System.Text.Json;

namespace mqtt.demo
{
    internal static class ObjectExtensions
    {
        public static TObject DumpToConsole<TObject>(this TObject @object)
        {
            var output = "NULL";
            if (@object != null)
            {
                output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }

            Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
            return @object;
        }

        public static string CenterString(this string stringToCenter, int totalLength)
        {
            return stringToCenter.PadLeft(((totalLength - stringToCenter.Length) / 2)
                                + stringToCenter.Length)
                       .PadRight(totalLength);
        }
    }
}
