using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TokenBasedAuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserCategory UserCategory { get; set; }
        [StringLength(25)]
        public string FirstName { get; set; }

        [StringLength(25)]
        public string SurName { get; set; }
    }
   
}