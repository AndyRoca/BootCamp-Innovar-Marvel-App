using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace ImportApiData
{
    public class CreateProductListItem : IPlugin
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
                Entity product = (Entity)context.InputParameters["Target"];

                try
                {
                    Functions functions = new Functions();

                    Entity PriceListItem = new Entity("productpricelevel");

                    PriceListItem["pricelevelid"] = functions.GetEntityReference(service, "pricelevel", new string[] { "pricelevelid" }, "name", "Test Price List");
                    PriceListItem["productid"] = functions.GetEntityReference(service, "product", new string[] { "productid" }, "name", product.Attributes["name"].ToString());
                    PriceListItem["uomid"] = functions.GetEntityReference(service, "uom", new string[] { "uomid" }, "name", "Comic");
                    PriceListItem["transactioncurrencyid"] = functions.GetEntityReference(service, "transactioncurrency", new string[] { "transactioncurrencyid" }, "currencyname", "Euro");
                    PriceListItem["quantitysellingcode"] = new OptionSetValue(1);
                    PriceListItem["pricingmethodcode"] = new OptionSetValue(1);
                    PriceListItem["amount"] = new Money(10);


                    Guid PriceListItemGuid = service.Create(PriceListItem);

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in CreateProduct.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("CreateProduct: {0}", ex.ToString());
                    throw;
                }

            }
        }

    }
}
