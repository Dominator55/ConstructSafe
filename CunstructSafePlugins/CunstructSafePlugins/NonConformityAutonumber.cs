using ConstructSafePlugins;
using ConstructSafeCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CunstructSafePlugins
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
                using (ConstructSafeContext ctx = new ConstructSafeContext(service))
                {
                    dkl_NonConformity target = ((Entity)context.InputParameters["Target"] as Entity).ToEntity<dkl_NonConformity>();
                    var autonumberEnvironmentVariable = ctx.EnvironmentVariableDefinitionSet.FirstOrDefault(x => x.DisplayName == "nonConformityAutonumber");
                    var autonumberValue = ctx.EnvironmentVariableValueSet.FirstOrDefault(x => x.EnvironmentVariableDefinitionId.Id == autonumberEnvironmentVariable.Id);
                    autonumberValue.Value = (int.Parse(autonumberValue.Value)+1).ToString();
                    target.dkl_Autonumber = int.Parse(autonumberValue.Value);
                    ctx.UpdateObject(autonumberValue);
                    ctx.SaveChanges();
                }
            }
            catch (InvalidPluginExecutionException e) { throw e; }
            catch (Exception e) { throw new InvalidPluginExecutionException(e.ToString()); }
        }
    }
}
