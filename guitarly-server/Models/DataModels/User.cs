using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace Models.DataModels
{
    [Table("users")]
    public class User: IdentityUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string Id { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool NotificationsEnabled { get; set; }
        public string ReferedFrom { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Photo100 { get; set; }
        public string Photo200 { get; set; }
        public string PhotoMaxOrig { get; set; }
        public int Sex { get; set; }
        
        public List<FavoriteSong> FavoriteSongs { get; set; }
    }
}