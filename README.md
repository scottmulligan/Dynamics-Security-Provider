# Microsoft Dynamics CRM Security Provider for Sitecore
Expose contacts from Microsoft Dynamics as Sitecore users, marketing lists as Sitecore roles and interact directly with Microsoft Dynamics.

------------------------

April, 2, 2018 - Migrated the existing codebase to Visual Studio 2015, updated .csproj files, moved references to nuget, and other housekeeping to get codebase to compile.

*Note that the Sitecore.Kernell reference is for Sitecore 8.2. You may need to update this nuget reference if working against other versions of Sitecore. Also not sure about the Dynamics CRM nuget references (Microsoft.Xrm.Sdk.2016 and UnOfficial.Microsoft.Crm.Sdk.Proxy nuget references) - I will add more information after reviewing assembly information for various version of this module.
