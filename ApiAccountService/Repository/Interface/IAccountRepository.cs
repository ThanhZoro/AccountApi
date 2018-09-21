using ApiAccountService.Models;
using Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ApiAccountService.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<UserViewModel> Create(CreateAccount data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<UserViewModel> Update(UpdateAccount data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task ResetPassword(ResetPasswordViewModel data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<IdentityResult> ChangePassword(ChangePasswordViewModel data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        Task<string> UploadAvatar(UserAvatar data, string updatedBy);
    }
}
