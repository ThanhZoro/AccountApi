using Microsoft.AspNetCore.Mvc;
using System;

namespace ApiAccountService.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper,string userId, string code)
        {
            var url = $"{Environment.GetEnvironmentVariable("IS_SERVER")}/Account/ResetPassword?userId={userId}&code={code}"; 
            return url;
        }
    }
}
