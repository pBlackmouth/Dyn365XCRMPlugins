using DynamicsCrm.Base.Plugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Package
{
    public class Plugin_CreateTask : PluginBase
    {
        
        public Plugin_CreateTask()
        {
            LogicalName = "lead";
        }

        protected override void Execute(IPluginExecutionContext context, IOrganizationService service, ITracingService tracingService)
        {

            try
            {
                Entity task = new Entity("task");
                task["subject"] = "Send e-mail to the new lead";
                task["description"] = "Follow up with the customer. Check if there are any new issues that need resolution.";
                task["scheduledstart"] = DateTime.Now.AddDays(7);
                task["scheduledend"] = DateTime.Now.AddDays(7);
                task["category"] = context.PrimaryEntityName;

                // Refer to the account in the task activity.
                if (context.OutputParameters.Contains("id"))
                {
                    Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                    string regardingobjectidType = LogicalName;

                    task["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                }

                service.Create(task);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in Plugin_CreateTask.", ex);
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
                throw new Exception(ex.Message, ex);
            }

        }
    }
}
