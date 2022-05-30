using System;
using System.Collections.Generic;
using Models.DataModels;

namespace Models.EntityModels
{
    public class UserEntity
    {
        public int Id { get; set; }
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
    }
}