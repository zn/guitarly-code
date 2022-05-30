using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.DataModels;
using Models.EntityModels;

namespace Models.ViewModels
{
    public class AuthViewModel 
    {

        [MaxLength(1000)]
        public string QueryString { get; set; }
        
        [MaxLength(200)]
        public string FirstName { get; set; }
        
        [MaxLength(200)]
        public string LastName { get; set; }
        
        [MaxLength(1000)]
        public string Photo100 { get; set; }
        [MaxLength(1000)]
        public string Photo200 { get; set; }
        [MaxLength(1000)]
        public string PhotoMaxOrig { get; set; }
        public int Sex { get; set; }
    }
}
