using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiAccountService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateAccount
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
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
        [StringLength(255)]
        [Phone(ErrorMessage = "phoneInvalid")]
        [Required(ErrorMessage = "phoneRequired")]
        public string PhoneNumber { get; set; }
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
        public string UpdatedBy { get; set; }
    }
}
