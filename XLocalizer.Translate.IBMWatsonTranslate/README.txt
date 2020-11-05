XLocalizer.Translate.IBMWatsonTranslate

Instructions to use this package :

- This package requires IBM Cloud Language Translator API Key, service name and url, must be obtained from https://cloud.ibm.com/catalog/services/language-translator
- Add the API key to user secrets :

````
{
  "XLocalizer.Translate": {
    "IBMWatsonTranslateApiKey": "xxx-imb-watson-cloud-api-key-xxx",
    "IBMWatsonTranslateServiceUrl": "https//ibm-service-instance-url",
    "IBMWatsonTranslateServiceVersionDate": "ibm-service-version-date"
  }
}
````

- Register in startup:
````
services.AddSingleton<ITranslator, IBMWatsonTranslateService>();
````

Repository: https://github.com/LazZiya/XLocalizer.Translate.IBMWatsonTranslate
Docs: https://docs.ziyad.info/en/XLocalizer/v1.0/translate-services-ibm.md