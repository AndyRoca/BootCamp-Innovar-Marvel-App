using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Net.Http;

namespace ImportApiData
{
    public class LowStockComicEmail : CodeActivity
    {
        [Input("arc_comicapiid")]
        public InArgument<string> arc_comicapiid { get; set; }

        [Output("CrmUrl")]
        public OutArgument<string> CrmUrl { get; set; }

        [Output("MarvelApiUrl")]
        public OutArgument<string> MarvelApiUrl { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string parameter = "comics";
            string variant = "noVariants=true";
            string timeStamp = "ts=1";
            string apiKey = "apikey=587b8fa26b0877e823e0ecfd50ced584";
            string hash = "hash=c762e0635cdc7430562e39c2b9ebf4d3";
            string baseUrl = "https://gateway.marvel.com/v1/public/";

            string comicapiid = arc_comicapiid.Get(executionContext);
            Guid guidCrmUrl = context.PrimaryEntityId;

            string MarvelUrl = baseUrl + parameter + "/" + comicapiid + "?" + variant + "&" + timeStamp + "&" + apiKey + "&" + hash;
            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, MarvelUrl);
            HttpResponseMessage response = client.GetAsync(MarvelUrl).Result;
            response.EnsureSuccessStatusCode();

            string responseContent = response.Content.ReadAsStringAsync().Result;
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

            string MarvelComicUrl = responseObject.data.results[0].urls[0].url;
            string CrmComicUrl = $"https://innovarxavi.crm4.dynamics.com/main.aspx?etn=arc_comic&id={guidCrmUrl}&pagetype=entityrecord";

            MarvelApiUrl.Set(executionContext, MarvelComicUrl);
            CrmUrl.Set(executionContext, CrmComicUrl);
        }
    }
}