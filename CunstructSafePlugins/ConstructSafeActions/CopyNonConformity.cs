using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstructSafeCommon;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace ConstructSafeActions
{
    public class CopyNonConformity : CodeActivity
    {

        [Input("NonConformityID")]
        [RequiredArgument]
        public InArgument<string> NonConformityID { get; set; }

        [Output("copyId")]
        public OutArgument<string> copyId { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            using (ConstructSafeContext ctx = new ConstructSafeContext(service))
            {
                try
                {
                    Guid nonConformityID = new Guid(NonConformityID.Get(executionContext));
                    tracingService.Trace("nonConformityID: {0}", nonConformityID);
                    var nonConformityCopy = ctx.dkl_NonConformitySet.FirstOrDefault(x => x.Id == nonConformityID);
                    nonConformityCopy.dkl_NonConformityId = null;
                    tracingService.Trace("nonConformityCopy filled with: {0}", nonConformityCopy.dkl_Name);
                    nonConformityCopy.Id = Guid.Empty;
                    tracingService.Trace("nonConformityCopy GUID filled with: {0}", nonConformityCopy.Id);
                    nonConformityCopy.dkl_Name = nonConformityCopy.dkl_Name + "_Copy";
                    tracingService.Trace("nonConformityCopy name filled with: {0}", nonConformityCopy.dkl_Name);
                    var nonConformityCopyID = service.Create(nonConformityCopy);
                    tracingService.Trace("new nonConformityCopy ID: {0}", nonConformityCopyID);
                    var activities = ctx.ActivityPointerSet.Where(x => x.RegardingObjectId.Id == nonConformityID);
                    tracingService.Trace("number of activities: {0}", activities.Count());
                    var activitiesCopy = new List<ActivityPointer>();
                    foreach(var activity in activities)
                    {
                        activity.Id = Guid.Empty;
                        activity.Subject = activity.Subject + "_Copy";
                        tracingService.Trace("copying activity with subject {0}", activity.Subject);
                        activity.RegardingObjectId.Id = nonConformityCopyID;
                        activitiesCopy.Add(activity);
                        ctx.UpdateObject(activity);
                    }
                    ctx.SaveChanges();
                    copyId.Set(executionContext, nonConformityCopyID);

                }
                catch (InvalidPluginExecutionException e) { throw e; }
                catch (Exception e) { throw new InvalidPluginExecutionException(e.ToString()); }
            }
        }
    }
}

