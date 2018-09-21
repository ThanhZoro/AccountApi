using ApiAccountService.Models;
using Contracts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiAccountService.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAccessRightRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<UserPermissions> Create(CreateAccessRight model);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<UserPermissions> Delete(DeleteAccessRight model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<string>> Roles(string companyId, string userId);
    }
}
