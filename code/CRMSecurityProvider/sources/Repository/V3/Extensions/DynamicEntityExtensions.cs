namespace CRMSecurityProvider.Sources.Repository.V3.Extensions
{
  using System;
  using System.Linq;
  using CRMSecurityProvider.crm3.webservice;

  public static class DynamicEntityExtensions
  {
     public static Guid GetId(this DynamicEntity dynamicEntity)
     {
       if (dynamicEntity.Properties == null || dynamicEntity.Properties.Length == 0)
       {
         return Guid.Empty;
       }

       var keyProperty = dynamicEntity.Properties.OfType<KeyProperty>().FirstOrDefault();
       if (keyProperty == null)
       {
         return Guid.Empty;
       }

       return keyProperty.Value.Value;
     }
  }
}