using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace ImportApiData
{
    public class CustomActionCall : IPlugin
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

            try
            {

                Functions customFunctions = new Functions();

                string MyInputParam = (string)context.InputParameters["MyInputParam"];

                Entity newComic = new Entity("arc_comic");

                newComic["arc_comicapiid"] = int.Parse(MyInputParam);
                newComic["arc_name"] = "New Comic";

                Guid newComicId = service.Create(newComic);

            }

            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in CustomActionCall.", ex);
            }

            catch (Exception ex)
            {
                tracingService.Trace("ImportData: {0}", ex.ToString());
                throw;
            }

        }
    }

}
