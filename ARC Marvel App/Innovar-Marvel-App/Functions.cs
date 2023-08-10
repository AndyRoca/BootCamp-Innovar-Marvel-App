using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace ImportApiData
{
    internal class Functions
    {
        private QueryExpression BuildQuery(string entityName, string[] column, string conditionColum, string conditionValue)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(column);
            query.Criteria.AddCondition(conditionColum, ConditionOperator.Equal, conditionValue);

            return query;
        }
        internal EntityReference GetEntityReference(IOrganizationService service, string entityName, string[] column, string conditionColum, string conditionValue)
        {
            QueryExpression query = BuildQuery(entityName, column, conditionColum, conditionValue);

            EntityCollection results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                Entity entity = results.Entities[0];
                return new EntityReference(entityName, entity.Id);
            }
            else
            {
                throw new InvalidPluginExecutionException($"The '{conditionValue}' could not be found.");
            }
        }

        internal string GetEntityAttribute(IOrganizationService service, string entityName, string[] column, string conditionColum, string conditionValue)
        {
            QueryExpression query = BuildQuery(entityName, column, conditionColum, conditionValue);
            EntityCollection results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                Entity entity = results.Entities[0];
                string attributeValue = entity.Attributes[column[0]].ToString();
                return attributeValue;
            }
            else
            {
                throw new InvalidPluginExecutionException($"Attribute '{column[0]}' has not been found.");
            }
        }
        internal Dictionary<string, object> GetEntityAttributes(IOrganizationService service, string entityName, string[] columns, string conditionColum, string conditionValue)
        {
            QueryExpression query = BuildQuery(entityName, columns, conditionColum, conditionValue);
            EntityCollection results = service.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
            {
                Entity entity = results.Entities[0];
                Dictionary<string, object> attributeValues = new Dictionary<string, object>();

                foreach (var column in columns)
                {
                    if (entity.Contains(column))
                    {
                        attributeValues.Add(column, entity.Attributes[column]);
                    }
                }

                return attributeValues;
            }
            else
            {
                throw new InvalidPluginExecutionException($"Attribute '{string.Join(",", columns)}' has not been found.");
            }
        }

        internal bool EntityExist(IOrganizationService service, string entityName, string[] column, string conditionColum, string conditionValue)
        {
            QueryExpression query = BuildQuery(entityName, column, conditionColum, conditionValue);

            EntityCollection results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        internal bool HasAdminRole(IOrganizationService service, Guid systemUserId, Guid AdminRoleTemplateId)
        {

            QueryExpression query = new QueryExpression("role");
            query.Criteria.AddCondition("roletemplateid", ConditionOperator.Equal, AdminRoleTemplateId);
            LinkEntity link = query.AddLink("systemuserroles", "roleid", "roleid");
            link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, systemUserId);

            return service.RetrieveMultiple(query).Entities.Count > 0;
        }
    }
}