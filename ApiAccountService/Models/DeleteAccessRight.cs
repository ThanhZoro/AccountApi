using Contracts.Commands;
using System.Collections.Generic;

namespace ApiAccountService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteAccessRight : IDeleteAccessRight
    {
        /// <summary>
        /// 
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> UserList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> RoleList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
