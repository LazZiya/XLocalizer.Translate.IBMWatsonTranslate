using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Cloud.SDK.Core.Http.Exceptions;
using IBM.Watson.LanguageTranslator.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace XLocalizer.Translate.IBMWatsonTranslate
{
    /// <summary>
    /// IBM Watson translation service for XLocalizer.TranslationServices
    /// </summary>
    public class IBMWatsonTranslateService : ITranslator
    {
        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName => "IBM Watson";

        private readonly ILogger _logger;

        private readonly LanguageTranslatorService _translatorService;

        /// <summary>
        /// Initialize new instance of IBM Watson trnaslation service
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IBMWatsonTranslateService(IConfiguration configuration, ILogger<IBMWatsonTranslateService> logger)
        {
            _logger = logger;

            var IBMWatsonTranslateApiKey = configuration["XLocalizer.Translate:IBMWatsonTranslateApiKey"] ?? throw new NullReferenceException("Configuration key for IBMWatsonTranslateApiKey was not found! For more details see https://docs.ziyad.info/en/XLocalizer/v1.0/translate-services-ibm.md");

            var IBMWatsonTranslateServiceUrl = configuration["XLocalizer.Translate:IBMWatsonTranslateServiceUrl"] ?? throw new NullReferenceException("Configuration key for IBMWatsonTranslateServiceUrl was not found! For more details see https://docs.ziyad.info/en/XLocalizer/v1.0/translate-services-ibm.md");

            var IBMWatsonTranslateServiceVersionDate = configuration["XLocalizer.Translate:IBMWatsonTranslateServiceVersionDate"] ?? throw new NullReferenceException("Configuration key for IBMWatsonTranslateServiceVersionDate was not found! For more details see https://docs.ziyad.info/en/XLocalizer/v1.0/translate-services-ibm.md");

            var authenticator = new IamAuthenticator(IBMWatsonTranslateApiKey);
            _translatorService = new LanguageTranslatorService(IBMWatsonTranslateServiceVersionDate, authenticator);

            _translatorService.SetServiceUrl(IBMWatsonTranslateServiceUrl);
            _translatorService.DisableSslVerification(true);
        }

        /// <summary>
        /// Translate text...
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="text"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Task<TranslationResult> TranslateAsync(string source, string target, string text, string format)
        {
            try
            {
                var result = _translatorService.Translate(new List<string> { text }, $"{source}-{target}");

                var transResult = new TranslationResult
                {
                    Text = result.Result.Translations[0]._Translation,
                    StatusCode = (HttpStatusCode)(int)result.StatusCode,
                    Target = target,
                    Source = source
                };

                return Task.FromResult<TranslationResult>(transResult);
            }
            catch (ServiceResponseException e)
            {
                _logger.LogError($"IBM Watson Translate Service Response Error: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"IBM Watson Translate Error: {e.Message}");
            }

            return Task.FromResult<TranslationResult>(new TranslationResult { StatusCode = HttpStatusCode.BadRequest, Source = source, Target = target, Text = text });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="text"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        public bool TryTranslate(string source, string target, string text, out string translation)
        {
            var trans = Task.Run(() => TranslateAsync(source, target, text, "text")).GetAwaiter().GetResult();

            if (trans.StatusCode == HttpStatusCode.OK)
            {
                translation = trans.Text;
                return true;
            }

            translation = text;
            return false;
        }
    }
}