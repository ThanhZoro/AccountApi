using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAccountService.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelStateErrorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static List<string> Error(this ModelStateDictionary modelState)
        {
            var data = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => x.ErrorMessage))
                .ToList();
            return data;
        }
    }
}
