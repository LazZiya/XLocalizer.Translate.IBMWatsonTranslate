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
    public class IBMWatsonStringTranslator : IStringTranslator
    {
        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName => "IBM Watson";

        private readonly ILogger _logger;
        
        private readonly string IBMWatsonTranslateApiKey;
        private readonly string IBMWatsonTranslateServiceUrl;
        private readonly string IBMWatsonTranslateServiceVersionDate;

        /// <summary>
        /// Initialize new instance of IBM Watson trnaslation service
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IBMWatsonStringTranslator(IConfiguration configuration, ILogger<IBMWatsonStringTranslator> logger)
        {
            _logger = logger;

            IBMWatsonTranslateApiKey = configuration["XLocalizer.TranslationServices:IBMWatsonTranslateApiKey"];
            if (string.IsNullOrWhiteSpace(IBMWatsonTranslateApiKey))
            {
                throw new NullReferenceException(nameof(IBMWatsonTranslateApiKey));
            }

            IBMWatsonTranslateServiceUrl = configuration["XLocalizer.TranslationServices:IBMWatsonTranslateServiceUrl"];
            if (string.IsNullOrWhiteSpace(IBMWatsonTranslateServiceUrl))
            {
                throw new NullReferenceException(nameof(IBMWatsonTranslateServiceUrl));
            }

            IBMWatsonTranslateServiceVersionDate = configuration["XLocalizer.TranslationServices:IBMWatsonTranslateServiceVersionDate"];
            if (string.IsNullOrWhiteSpace(IBMWatsonTranslateServiceVersionDate))
            {
                throw new NullReferenceException(nameof(IBMWatsonTranslateServiceVersionDate));
            }
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
                var authenticator = new IamAuthenticator(IBMWatsonTranslateApiKey);
                var languageTranslator = new LanguageTranslatorService(IBMWatsonTranslateServiceVersionDate, authenticator);

                languageTranslator.SetServiceUrl(IBMWatsonTranslateServiceUrl);
                languageTranslator.DisableSslVerification(true);

                var result = languageTranslator.Translate(new List<string> { text }, $"{source}-{target}");

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