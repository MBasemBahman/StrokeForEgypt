using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.AccountEntity
{
    public class AccountPayment : BaseEntity
    {
        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public Account Account { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Reference Number")]
        public string ReferenceNumber { get; set; }

        [DisplayName("Merchant Ref Number")]
        public string MerchantRefNumber { get; set; }

        [DisplayName("Order Amount")]
        public double OrderAmount { get; set; }

        [DisplayName("Payment Amount")]
        public double PaymentAmount { get; set; }

        [DisplayName("Fawry Fees")]
        public double FawryFees { get; set; }

        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; }

        [DisplayName("Order Status")]
        public string OrderStatus { get; set; }

        [DisplayName("Payment Time")]
        public string PaymentTime { get; set; }

        [DisplayName("Customer Mobile")]
        public string CustomerMobile { get; set; }

        [DisplayName("Customer Mail")]
        public string CustomerMail { get; set; }

        [DisplayName("Customer Profile Id")]
        public string CustomerProfileId { get; set; }

        [DisplayName("Signature")]
        public string Signature { get; set; }

        [DisplayName("Status Code")]
        public int StatusCode { get; set; }

        [DisplayName("Status Description")]
        public string StatusDescription { get; set; }

        [DisplayName("Booking")]
        public Booking Booking { get; set; }
    }
}
