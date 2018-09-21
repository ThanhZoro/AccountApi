using System.Collections.Generic;
using Contracts.Commands;

namespace ApiAccountService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateAccessRight : ICreateAccessRight
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
