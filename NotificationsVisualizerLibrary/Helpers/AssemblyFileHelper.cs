using System.IO;
using System.Reflection;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal static class AssemblyFileHelper
    {
        /// <summary>
        /// fileName should be formatted with "." instead of "/" for folder paths
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetResourceText(string fileName)
        {
            // This code came from here: https://github.com/matthidinger/CardedUrls/blob/1aec9ad4e79d84914bfe648673d937ddae88486c/source/CardedUrls.Core/Models/AdaptiveCardTemplate.cs#L60
            string resourceName = $"NotificationsVisualizerLibrary.{fileName}";

            using Stream? stream = typeof(AssemblyFileHelper).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
            using StreamReader streamReader = new(stream!);
            return streamReader.ReadToEnd();
        }
    }
}
