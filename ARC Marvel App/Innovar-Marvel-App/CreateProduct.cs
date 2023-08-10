using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace ImportApiData
{
    public class CreateProduct : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Extract the tracing service for use in debugging sandboxed plug-ins.  
            // If you are not registering the plug-in in the sandbox, then you do  
            // not have to add any tracing service related code.  
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            // Obtain the organization service reference which you will need for  
            // web service calls.  
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                try
                {
                    Functions customFunctions = new Functions();

                    Entity product = new Entity("product");

                    object name = entity.Attributes;

                    product["name"] = customFunctions.GetEntityAttribute(service, "arc_comic", new string[] { "arc_name" }, "arc_comicid", entity.Id.ToString());
                    product["productnumber"] = customFunctions.GetEntityAttribute(service, "arc_comic", new string[] { "arc_comicapiid" }, "arc_comicid", entity.Id.ToString());
                    product["defaultuomscheduleid"] = customFunctions.GetEntityReference(service, "uomschedule", new string[] { "uomscheduleid" }, "name", "Comic");
                    product["defaultuomid"] = customFunctions.GetEntityReference(service, "uom", new string[] { "uomid" }, "name", "Comic");
                    product["quantitydecimal"] = 2;

                    Guid createdProductId = service.Create(product);
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }

    }
}