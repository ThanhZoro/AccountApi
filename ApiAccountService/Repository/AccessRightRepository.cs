using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiAccountService.Data;
using ApiAccountService.Extensions;
using ApiAccountService.Models;
using Contracts.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nest;

namespace ApiAccountService.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessRightRepository : IAccessRightRepository
    {
        private ApplicationDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public AccessRightRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserPermissions> Create(CreateAccessRight model)
        {
            var data = new UserPermissions() { UserRoles = new List<UserRoles>(), AccessRights = new List<AccessRight>() };
            foreach (var item in model.UserList)
            {
                var newData = await _context.AccessRight.Find(f => f.UserId == item && f.CompanyId == model.CompanyId).FirstOrDefaultAsync();
                if (newData != null)
                {
                    var roleList = newData.RoleList ?? new List<string>();
                    var newRoleList = roleList.Union(model.RoleList).ToList();

                    var filter = Builders<AccessRight>.Filter.Eq("Id", newData.Id);
                    var update = Builders<AccessRight>.Update
                        .Set(s => s.RoleList, newRoleList)
                        .Set(s => s.UpdatedBy, model.CreatedBy)
                    .CurrentDate(s => s.UpdatedAt);
                    var options = new FindOneAndUpdateOptions<AccessRight>
                    {
                        ReturnDocument = ReturnDocument.After
                    };
                    newData = await _context.AccessRight.FindOneAndUpdateAsync(filter, update, options);
                    data.AccessRights.Add(newData);
                    data.UserRoles.Add(new UserRoles() { UserId = item, Roles = newRoleList });
                }
                else
                {
                    newData = new AccessRight
                    {
                        CreatedBy = model.CreatedBy,
                        CreatedAt = DateTime.UtcNow,
                        CompanyId = model.CompanyId,
                        UserId = item,
                        RoleList = model.RoleList
                    };
                    await _context.AccessRight.InsertOneAsync(newData);
                    data.AccessRights.Add(newData);
                    data.UserRoles.Add(new UserRoles() { UserId = item, Roles = model.RoleList });
                }
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserPermissions> Delete(DeleteAccessRight model)
        {
            var data = new UserPermissions() { UserRoles = new List<UserRoles>(), AccessRights = new List<AccessRight>() };
            foreach (var item in model.UserList)
            {
                var newData = await _context.AccessRight.Find(f => f.UserId == item && f.CompanyId == model.CompanyId).FirstOrDefaultAsync();
                if (newData != null)
                {
                    var roleList = newData.RoleList ?? new List<string>();
                    var newRoleList = roleList.Where(s => !model.RoleList.Contains(s)).ToList();

                    var filter = Builders<AccessRight>.Filter.Eq("Id", newData.Id);
                    var update = Builders<AccessRight>.Update
                        .Set(s => s.RoleList, newRoleList)
                        .Set(s => s.UpdatedBy, model.CreatedBy)
                    .CurrentDate(s => s.UpdatedAt);
                    var options = new FindOneAndUpdateOptions<AccessRight>
                    {
                        ReturnDocument = ReturnDocument.After
                    };
                    newData = await _context.AccessRight.FindOneAndUpdateAsync(filter, update, options);
                    data.AccessRights.Add(newData);
                    data.UserRoles.Add(new UserRoles() { UserId = item, Roles = newRoleList });
                }
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> Roles(string companyId, string userId)
        {
            var userRoles = new List<string>();
            var accessRight = await _context.AccessRight.Find(f => f.CompanyId == companyId && f.UserId == userId).FirstOrDefaultAsync();
            if (accessRight != null)
            {
                userRoles = accessRight.RoleList;
            }
            return userRoles;
        }
    }
}
