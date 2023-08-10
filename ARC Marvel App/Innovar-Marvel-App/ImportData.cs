using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace ImportApiData
{
    public class ImportData : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                try
                {

                    HttpClient client = new HttpClient();

                    string parameter = "comics";
                    string variant = "noVariants=true";
                    string timeStamp = "ts=1";
                    string apiKey = "apikey=587b8fa26b0877e823e0ecfd50ced584";
                    string hash = "hash=c762e0635cdc7430562e39c2b9ebf4d3";
                    string baseUrl = "https://gateway.marvel.com/v1/public/";
                    string url = baseUrl + parameter + "?" + variant + "&" + timeStamp + "&" + apiKey + "&" + hash;

                    string comicNameInput = entity.Attributes["arc_name"].ToString();

                    string titlePattern = @"^(.*?)(?=\s*\()";
                    string yearPattern = @"\((\d{4})\)";
                    string issuePattern = @"#(\d+)";

                    Match matchTitle = Regex.Match(comicNameInput, titlePattern);
                    Match matchYear = Regex.Match(comicNameInput, yearPattern);
                    Match matchIssue = Regex.Match(comicNameInput, issuePattern);

                    string title = matchTitle.Groups[1].Value;
                    string year = matchYear.Groups[1].Value;
                    string issue = matchIssue.Groups[1].Value;

                    if (!string.IsNullOrEmpty(title))
                    {
                        url = url + "&titleStartsWith=" + title;
                    }

                    if (string.IsNullOrEmpty(title))
                    {
                        url = url + "&titleStartsWith=" + comicNameInput;
                    }

                    if (!string.IsNullOrEmpty(year))
                    {
                        url = url + "&startYear=" + year;
                    }

                    if (!string.IsNullOrEmpty(issue))
                    {
                        url = url + "&issueNumber=" + issue;
                    }

                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();

                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                    if (responseObject.data.count == 0)
                    {
                        string id = entity.Attributes["arc_comicapiid"].ToString();
                        url = baseUrl + parameter + "/" + id + "?" + variant + "&" + timeStamp + "&" + apiKey + "&" + hash;

                        var requestForId = new HttpRequestMessage(HttpMethod.Get, url);
                        HttpResponseMessage responseForId = client.GetAsync(url).Result;
                        responseForId.EnsureSuccessStatusCode();

                        string responseContentForId = responseForId.Content.ReadAsStringAsync().Result;
                        dynamic responseObjectForId = JsonConvert.DeserializeObject(responseContentForId);

                        string comicTitleForId = responseObjectForId.data.results[0].title;
                        int comicIdForId = responseObjectForId.data.results[0].id;
                        string descriptionForId = responseObjectForId.data.results[0].description;

                        string ImageUrlForId = responseObjectForId.data.results[0].thumbnail.path + "." + responseObjectForId.data.results[0].thumbnail.extension;


                        if (string.IsNullOrEmpty(descriptionForId))
                        {
                            descriptionForId = "No tiene descripción";
                        }

                        entity["arc_name"] = comicTitleForId;
                        entity["arc_comicapiid"] = comicIdForId;
                        entity["arc_description"] = descriptionForId;
                        entity["arc_imageurl"] = ImageUrlForId;
                        service.Update(entity);

                    }
                    else
                    {
                        string comicTitle = responseObject.data.results[0].title;
                        int comicId = responseObject.data.results[0].id;
                        string description = responseObject.data.results[0].description;
                        string ImageUrl = responseObject.data.results[0].thumbnail.path + "." + responseObject.data.results[0].thumbnail.extension;

                        if (string.IsNullOrEmpty(description))
                        {
                            description = "No tiene descripción";
                        }

                        entity["arc_name"] = comicTitle;
                        entity["arc_comicapiid"] = comicId;
                        entity["arc_description"] = description;
                        entity["arc_imageurl"] = ImageUrl;
                        service.Update(entity);
                    }

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in ImportData.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("ImportData: {0}", ex.ToString());
                    throw;
                }

            }
        }

    }
}