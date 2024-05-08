using ConstructSafeCommon;
using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace ConstructSafePlugins
{
    public class NonConformityAutonumber : PluginBase
    {
        public NonConformityAutonumber(string unsecureConfiguration, string secureConfiguration)
           : base(typeof(NonConformityAutonumber))
        {
        }
        protected override void ExecuteCrmPlugin(LocalPluginContext localPluginContext)
        {
            if (localPluginContext == null) { throw new ArgumentNullException("localPluginContext"); }

            IOrganizationService service = localPluginContext.OrganizationService;
            IPluginExecutionContext context = localPluginContext.PluginExecutionContext;
            ITracingService tracingService = localPluginContext.TracingService;

            if (context.Depth > 1) { return; }

            Guid primaryEntityId = context.PrimaryEntityId;
            tracingService.Trace("Primary entity ID: {0}", primaryEntityId);
            string primaryEntityName = context.PrimaryEntityName;
            try
            {
                if (context.MessageName == PluginMessage.Create && context.Stage == StageNum.Pre_Operation) { 
                    using (ConstructSafeContext ctx = new ConstructSafeContext(service))
                    {
                        dkl_NonConformity target = ((Entity)context.InputParameters["Target"] as Entity).ToEntity<dkl_NonConformity>();
                        var autonumberEnvironmentVariable = ctx.EnvironmentVariableDefinitionSet.FirstOrDefault(x => x.DisplayName == "nonConformityAutonumber");
                        var autonumberValue = ctx.EnvironmentVariableValueSet.FirstOrDefault(x => x.EnvironmentVariableDefinitionId.Id == autonumberEnvironmentVariable.Id);
                        tracingService.Trace("Current Autonumber value", autonumberValue.Value);
                        autonumberValue.Value = (int.Parse(autonumberValue.Value) + 1).ToString();
                        tracingService.Trace("New Autonumber value", autonumberValue.Value);
                        target.dkl_Autonumber = int.Parse(autonumberValue.Value);
                        tracingService.Trace("Set the autonumber value to the target.");
                        ctx.UpdateObject(autonumberValue);
                        ctx.SaveChanges();
                        tracingService.Trace("Changes to EnvVar saved.");
                    }
                }
            }
            catch (InvalidPluginExecutionException e) { throw e; }
            catch (Exception e) { throw new InvalidPluginExecutionException(e.ToString()); }
        }
    }
}
