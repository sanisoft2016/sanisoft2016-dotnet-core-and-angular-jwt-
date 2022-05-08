using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TokenBasedAuthAPI.Models
{
    public class PaymentDetails
    {
        [Key]
        public int PaymentDetailId { set; get; }
        [StringLength(60)]
        public string CardOwnerName { set; get; }

        [StringLength(16)]
        public string CardNumber { set; get; }

        [StringLength(5)]
        public string ExpiryDate { set; get; }
        [StringLength(5)]
        public string SecurityCode { set; get; }

    }
}
