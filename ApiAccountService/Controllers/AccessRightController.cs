using System.Linq;
using System.Threading.Tasks;
using ApiAccountService.Models;
using Contracts.Commands;
using Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiAccountService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/accounts/accessRight")]
    public class AccessRightController : Controller
    {
        private readonly IRequestClient<ICreateAccessRight, UserPermissions> _createAccessRightRequestClient;
        private readonly IRequestClient<IDeleteAccessRight, UserPermissions> _deleteAccessRightRequestClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createAccessRightRequestClient"></param>
        /// <param name="deleteAccessRightRequestClient"></param>
        /// <param name="httpContextAccessor"></param>
        public AccessRightController(IRequestClient<ICreateAccessRight, UserPermissions> createAccessRightRequestClient,
            IRequestClient<IDeleteAccessRight, UserPermissions> deleteAccessRightRequestClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _createAccessRightRequestClient = createAccessRightRequestClient;
            _deleteAccessRightRequestClient = deleteAccessRightRequestClient;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody]CreateAccessRight model)
        {
            var companyId = _httpContextAccessor.HttpContext.Request?.Headers["CompanyId"].FirstOrDefault();
            var createdBy = User.Claims.FirstOrDefault(s => s.Type == "userName").Value;

            model.CompanyId = companyId;
            model.CreatedBy = createdBy;
            var data = await _createAccessRightRequestClient.Request(model);
            return Ok(data.UserRoles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody]DeleteAccessRight model)
        {
            var companyId = _httpContextAccessor.HttpContext.Request?.Headers["CompanyId"].FirstOrDefault();
            var createdBy = User.Claims.FirstOrDefault(s => s.Type == "userName").Value;

            model.CompanyId = companyId;
            model.CreatedBy = createdBy;
            var data = await _deleteAccessRightRequestClient.Request(model);
            return Ok(data.UserRoles);
        }
    }
}