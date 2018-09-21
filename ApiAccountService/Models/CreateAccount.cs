using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiAccountService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateAccount
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        [Required(ErrorMessage = "firstNameRequired")]
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Position { get; set; } = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string Gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        [Phone(ErrorMessage = "phoneInvalid")]
        [Required(ErrorMessage = "phoneRequired")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "userNameInvalid")]
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool RequiredChangePassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Department { get; set; } = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSendMailRegister { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LinkLogin { get; set; }
    }
}
