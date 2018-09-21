using ApiAccountService.Extensions;
using ApiAccountService.Models;
using Contracts.Commands;
using Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAccountService.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccessRightRepository _accessRightRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="publishEndpoint"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="accessRightRepository"></param>
        public AccountRepository(
            UserManager<ApplicationUser> userManager, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor, IAccessRightRepository accessRightRepository)
        {
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
            _accessRightRepository = accessRightRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<UserViewModel> Create(CreateAccount data)
        {
            var user = new ApplicationUser
            {
                UserName = data.UserName,
                Email = data.UserName,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Position = data.Position,
                Gender = data.Gender,
                Birthday = data.Birthday,
                PhoneNumber = data.PhoneNumber,
                Code = data.Code,
                Address = data.Address,
                Department = data.Department,
                IsActive = true,
                Companies = new List<string>() { data.CompanyId },
                RequiredChangePassword = data.RequiredChangePassword,
                CreatedBy = data.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, data.Password);
            if (result.Succeeded)
            {
                var response = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Position = user.Position,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    Code = user.Code,
                    Department = user.Department,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    CreatedBy = user.CreatedBy,
                    UpdatedAt = user.UpdatedAt,
                    UpdatedBy = user.UpdatedBy,
                    RequiredChangePassword = user.RequiredChangePassword
                };
                if (data.IsSendMailRegister)
                {
                    var appDomain = Environment.GetEnvironmentVariable("APP_DOMAIN");
                    data.LinkLogin = $"http://{data.CompanyCode}.{appDomain}";
                    await _publishEndpoint.Publish<ISendMail>(
                            new
                            {
                                TypeNotification = TypeNotification.RegisterAccount,
                                data.Culture,
                                ObjectId = user.Id,
                                ObjectType = "users",
                                Data = new DataSendMail()
                                {
                                    Receiver = user.Email,
                                    Body = new
                                    {
                                        user.UserName,
                                        data.Password,
                                        FullName = user.FirstName + " " + user.LastName,
                                        data.CompanyName,
                                        user.Position,
                                        user.PhoneNumber,
                                        user.Email,
                                        data.LinkLogin
                                    }
                                }
                            });
                }
                return response;
            }
            else
            {
                throw new Exception(result.Errors.FirstOrDefault().Code);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<UserViewModel> Update(UpdateAccount data)
        {
            var user = _userManager.Users.FirstOrDefault(f => f.Id == data.Id);
            user.PhoneNumber = data.PhoneNumber;
            user.FirstName = data.FirstName;
            user.LastName = data.LastName;
            user.Department = data.Department;
            user.Position = data.Position;
            user.Gender = data.Gender;
            user.Birthday = data.Birthday;
            user.Code = data.Code;
            user.Address = data.Address;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = data.UpdatedBy;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var companyId = _httpContextAccessor.HttpContext.Request?.Headers["CompanyId"].FirstOrDefault();
                var userRole = await _accessRightRepository.Roles(companyId, user.Id);
                var response = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Position = user.Position,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    Code = user.Code,
                    Department = user.Department,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    CreatedBy = user.CreatedBy,
                    UpdatedAt = user.UpdatedAt,
                    UpdatedBy = user.UpdatedBy,
                    RequiredChangePassword = user.RequiredChangePassword,
                    Roles = userRole
                };
                return response;
            }
            else
            {
                throw new Exception(result.Errors.FirstOrDefault().Code);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ResetPassword(ResetPasswordViewModel data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<IdentityResult> ChangePassword(ChangePasswordViewModel data)
        {
            var user = _userManager.Users.FirstOrDefault(f => f.Id == data.UserId);
            user.RequiredChangePassword = false;
            await _userManager.UpdateAsync(user);
            var result = await _userManager.ChangePasswordAsync(user, data.OldPassword, data.NewPassword);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<string> UploadAvatar(UserAvatar data, string updatedBy)
        {
            string avatarUrl = string.Empty;
            if (data.File != null)
            {
                var uploadResult = CloudinaryUploadExtensions.UploadImageUser(data.File);
                avatarUrl = uploadResult.SecureUri.OriginalString;
            }
            var applicationUser = _userManager.Users.FirstOrDefault(s => s.Id == data.Id);
            if (applicationUser != null)
            {
                applicationUser.UpdatedAt = DateTime.UtcNow;
                applicationUser.UpdatedBy = updatedBy;
                applicationUser.AvatarUrl = avatarUrl;
                var result = await _userManager.UpdateAsync(applicationUser);
            }
            return avatarUrl;
        }
    }
}
