using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using e_loanApp.Models;
using System.Web.Security;

namespace e_loanApp.Models
{
    public class WebRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var context = new eLoan_ApplicationEntities1())
            {
                System.Diagnostics.Debug.WriteLine(username);
                var result = (from user in context.UserTables
                              join roleMapping in context.RoleMappingTables
                              on user.UserId equals roleMapping.UserId
                              join role in context.RoleTables
                               on roleMapping.RoleId equals role.RoleId
                              where user.Email_Address == username
                              select role.Role).ToArray();
                System.Diagnostics.Debug.WriteLine(result);
                return result;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}