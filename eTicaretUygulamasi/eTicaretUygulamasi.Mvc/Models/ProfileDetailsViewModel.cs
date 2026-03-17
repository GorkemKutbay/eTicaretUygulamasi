using eTicaretUygulamasi.Mvc.App.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class ProfileDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; } 
        public RoleEntity Role { get; set; }
    }
}
