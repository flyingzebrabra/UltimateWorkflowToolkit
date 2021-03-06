﻿using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using UltimateWorkflowToolkit.Common;

namespace UltimateWorkflowToolkit.CoreOperations.Security
{
    public class IsUserHasRole : CrmWorkflowBase
    {
        #region Input/Output Parameters

        [Input("User")]
        [RequiredArgument]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> User { get; set; }

        [Input("Security Role")]
        [RequiredArgument]
        [ReferenceTarget("role")]
        public InArgument<EntityReference> Role { get; set; }

        [Output("Is User Has Role")]
        public OutArgument<bool> HasRole { get; set; }

        #endregion Input/Output Parameters

        protected override void ExecuteWorkflowLogic()
        {
            var userQuery = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet(false)
            };
            userQuery.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, User.Get(Context.ExecutionContext).Id);

            var roleLink = userQuery.AddLink("systemuserroles", "systemuserid", "systemuserid");
            roleLink = roleLink.AddLink("role", "roleid", "roleid");
            roleLink.LinkCriteria.AddCondition("parentrootroleid", ConditionOperator.Equal, Role.Get(Context.ExecutionContext).Id);

            var hasRole = Context.SystemService.RetrieveMultiple(userQuery).Entities.Count != 0;

            HasRole.Set(Context.ExecutionContext, hasRole);
        }

    }
}
