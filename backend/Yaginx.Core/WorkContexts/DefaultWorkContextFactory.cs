using AgileLabs.WorkContexts;

namespace WoLabs.WorkContexts
{
    public class DefaultWorkContextFactory : DefaultWorkContextCoreFactory<DefaultWorkContext>
    {
        protected override DefaultWorkContext CreateWorkContext(IServiceProvider serviceProvider, DefaultWorkContext parentWorkContext = null, uint inheritFlags = 0xFFFFFFFF, PropertiesAssignMode assignMode = PropertiesAssignMode.Reference)
        {
            var newWorkContext = base.CreateWorkContext(serviceProvider, parentWorkContext, inheritFlags, assignMode);
            InheritParentProperties(parentWorkContext, (PropertiesInheritFlag)inheritFlags, newWorkContext, assignMode);
            return newWorkContext;
        }

        private void InheritParentProperties(
            DefaultWorkContext sourceWorkContext,
            PropertiesInheritFlag inheritFlags,
            DefaultWorkContext targetWorkContext,
            PropertiesAssignMode assignMode)
        {
            if (sourceWorkContext == null)
            {
                return;
            }
            //if ((inheritFlags & PropertiesInheritFlag.Tenant) == PropertiesInheritFlag.Tenant)
            //{
            //    targetWorkContext.Tenant = sourceWorkContext.Tenant.CloneByAssignMode(assignMode);
            //}
        }
    }
}
