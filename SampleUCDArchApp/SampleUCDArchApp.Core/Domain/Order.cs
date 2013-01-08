using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using UCDArch.Core.DomainModel;
using UCDArch.Core.Utils;

namespace SampleUCDArchApp.Core.Domain
{
    public class Order : DomainObject
    {
        /// <summary>
        /// This is a placeholder constructor for NHibernate.
        /// A no-argument constructor must be avilable for NHibernate to create the object.
        /// </summary>
        public Order() { }

        public Order(Customer orderedBy)
        {
            Check.Require(orderedBy != null, "orderedBy may not be null");

            OrderedBy = orderedBy;
        }

        [JsonProperty]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime OrderDate { get; set; }

        [Required]
        [StringLength(12)]
        [JsonProperty]
        public virtual string ShipAddress { get; set; }

        [Required]
        public virtual Customer OrderedBy { get; set; }
    }
}