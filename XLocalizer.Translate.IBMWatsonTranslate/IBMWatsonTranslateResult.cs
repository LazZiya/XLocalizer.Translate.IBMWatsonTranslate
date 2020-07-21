using Newtonsoft.Json;

namespace XLocalizer.Translate.IBMWatsonTranslate
{
    /// <summary>
    /// IBM Watson translate result
    /// </summary>
    public class IBMWatsonTranslateResult
    {
        /// <summary>
        /// Translations list
        /// </summary>
        [JsonProperty("translations")]
        public IBMWatsonTranslation[] Translations { get; set; }
    }

    /// <summary>
    /// Translation text object
    /// </summary>
    public class IBMWatsonTranslation
    {
        /// <summary>
        /// Translation text
        /// </summary>
        [JsonProperty("translation")]
        public string Translation { get; set; }
    }
}
