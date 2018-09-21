using System.Linq;
using System.Threading.Tasks;
using ApiAccountService.Models;
using ApiAccountService.Repository;
using Contracts.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ApiAccountService.Controllers
{
    /// <summary>
    /// summary for AccountsController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountsController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// contructor AccountsController
        /// </summary>
        /// <param name="accountRepository"></param>
        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// create user
        /// </summary>
        /// <param name="user"> user info</param>
        /// <param name="CompanyId">company id from header</param>
        /// <returns>the user created</returns>
        /// <response code="200">returns the user created</response>
        [HttpPut]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        public async Task<IActionResult> Create([FromBody]CreateAccount user, [FromHeader]string CompanyId)
        {
            if (ModelState.IsValid)
            {
                user.CreatedBy = User.Claims.FirstOrDefault(s => s.Type == "userName").Value;
                user.CompanyId = CompanyId;
                var response = await _accountRepository.Create(user);
                return Ok(response);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// update user
        /// </summary>
        /// <param name="user">user info</param>
        /// <returns>the user updated</returns>
        /// <response code="200">returns the user updated</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        public async Task<IActionResult> Update([FromBody]UpdateAccount user)
        {
            if (ModelState.IsValid)
            {
                user.UpdatedBy = User.Claims.FirstOrDefault(s => s.Type == "userName").Value;
                var response = await _accountRepository.Update(user);
                return Ok(response);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// reset password
        /// </summary>
        /// <param name="data">reset password info</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                await _accountRepository.ResetPassword(data);
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// change password
        /// </summary>
        /// <param name="data">change password info</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(s => s.Type == "sub").Value;
                data.UserId = userId;
                var identityResult = await _accountRepository.ChangePassword(data);
                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(identityResult.Errors);
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// upload avatar user
        /// </summary>
        /// <param name="data">upload avatar info</param>
        /// <returns>the user uploaded</returns>
        /// <response code="200">returns the user uploaded</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> UploadAvatar([FromForm]UserAvatar data)
        {
            if (ModelState.IsValid)
            {
                var updatedBy = User.Claims.FirstOrDefault(s => s.Type == "userName").Value;
                var result = await _accountRepository.UploadAvatar(data, updatedBy);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }
    }
}
