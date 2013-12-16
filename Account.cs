namespace Magentrix.Entities
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Account
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string OwnerId { get; set; }

        [DataMember]
        public string CreatedById { get; set; }

        [DataMember]
        public string ModifiedById { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Boolean? Active { get; set; }

        [DataMember]
        public string Web { get; set; }

        [DataMember]
        public string Password { get; set; }

        // If other account fields are needed, they can be added as required.
    }
}
