using System;
using Microsoft.Xrm.Sdk;
using DynamicsCrm.Base.Extensions;
using System.ServiceModel;

namespace DynamicsCrm.Base.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        protected string LogicalName = "";
        protected Entity entity = null;

        
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                //TODO: usar LogicalName != null && LogicalName != "", y despues cambiar por una extension de string.
                if (LogicalName.IsNullOrEmpty())
                {
                    throw new NullReferenceException("Entity LogicalName is not defined as parameter into Plugin Base constructor");
                }                

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.
                    Entity entity = (Entity)context.InputParameters["Target"];

                    // Verify that the target entity represents an account.
                    // If not, this plug-in was not registered correctly.
                    if (entity.LogicalName != LogicalName)
                        return;

                    // Obtain the organization service reference.
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


                    Execute(context, service, tracingService);

                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in PluginBase.", ex);
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        protected abstract void Execute(IPluginExecutionContext context, IOrganizationService service, ITracingService tracingService);
    }
}
