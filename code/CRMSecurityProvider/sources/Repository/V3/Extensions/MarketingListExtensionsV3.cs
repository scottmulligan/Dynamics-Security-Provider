using CRMSecurityProvider.crm3.webservice;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMSecurityProvider.Sources.Repository.V3.Extensions
{
    public static class MarketingListExtensions
    {
        public static AttributeCollection GetAttributes(this list list)
        {
            var attributes = new AttributeCollection();
            attributes.Add(nameof(list.cost), list.cost);
            attributes.Add(nameof(list.createdby), list.createdby);
            attributes.Add(nameof(list.createdfromcode), list.createdfromcode);
            attributes.Add(nameof(list.createdon), list.createdon);
            attributes.Add(nameof(list.description), list.description);
            attributes.Add(nameof(list.donotsendonoptout), list.donotsendonoptout);
            attributes.Add(nameof(list.ignoreinactivelistmembers), list.ignoreinactivelistmembers);
            attributes.Add(nameof(list.lastusedon), list.lastusedon);
            attributes.Add(nameof(list.listid), list.listid);
            attributes.Add(nameof(list.listname), list.listname);
            attributes.Add(nameof(list.lockstatus), list.lockstatus);
            attributes.Add(nameof(list.membercount), list.membercount);
            attributes.Add(nameof(list.membertype), list.membertype);
            attributes.Add(nameof(list.modifiedby), list.modifiedby);
            attributes.Add(nameof(list.modifiedon), list.modifiedon);
            attributes.Add(nameof(list.ownerid), list.ownerid);
            attributes.Add(nameof(list.owningbusinessunit), list.owningbusinessunit);
            attributes.Add(nameof(list.purpose), list.purpose);
            attributes.Add(nameof(list.source), list.source);
            attributes.Add(nameof(list.statecode), list.statecode);
            attributes.Add(nameof(list.statuscode), list.statuscode);
            return attributes;
        }
    }
}
