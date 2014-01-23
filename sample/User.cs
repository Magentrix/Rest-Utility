namespace Magentrix.Entities
{
    using System;
    using System.Runtime.Serialization;

    public class User
    {
       
        public string Id { get; set; }

        public string Name { get; set; }
        
        public string Username { get; set; }
        
        public string Firstname { get; set; }

        public string Lastname { get; set; }
        
        public string Email { get; set; }
        
        public string Alias { get; set; }
        
        public bool? IsActive { get; set; }
        
        public string Color { get; set; }
        
        public bool? AgreedToTerms { get; set; }
        
        public string RoleId { get; set; }
        
        public string Timezone { get; set; }

        public string Language { get; set; }
        
        public string Locale { get; set; }

        public bool? DisableRecordOwner { get; set; }
        
        public DateTime? LastLoginDate { get; set; }

        public virtual string CreatedById { get; set; }

        public virtual string ModifiedById { get; set; }

        public virtual DateTime? CreatedOn { get; set; }

        public virtual DateTime? ModifiedOn { get; set; }
    }
}
