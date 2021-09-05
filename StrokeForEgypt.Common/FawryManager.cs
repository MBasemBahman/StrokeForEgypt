using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace StrokeForEgypt.Common
{
    public class FawryManager
    {
        private readonly string MerchantCode;
        private readonly string SecureKey;

        private readonly string ScriptUrl;
        private readonly string StylesheetUrl;

        private readonly string ReturnUrl;

        private readonly string StatusUrl;

        public FawryManager(bool Production = true)
        {
            if (Production)
            {
                MerchantCode = "/PEnYvx9uDal787RKK+LMw==";
                SecureKey = "e6155f46e5ae4f4c8eb0c9423224c526";

                ScriptUrl = "https://www.atfawry.com/atfawry/plugin/assets/payments/js/fawrypay-payments.js";
                StylesheetUrl = "https://www.atfawry.com/atfawry/plugin/assets/payments/css/fawrypay-payments.css";

                ReturnUrl = "https://strokeforegyptapi.azurewebsites.net/fawry/ChargeResponse";

                StatusUrl = "https://atfawry.fawrystaging.com/ECommerceWeb/Fawry/payments/status/v2";
            }
            else
            {
                MerchantCode = "1tSa6uxz2nRo3lV+rfO2qg==";
                SecureKey = "6b1684246a2f44f682f24f039fc0d8e6";

                ScriptUrl = "https://atfawry.fawrystaging.com/atfawry/plugin/assets/payments/js/fawrypay-payments.js";
                StylesheetUrl = "https://www.atfawry.com/atfawry/plugin/assets/payments/css/fawrypay-payments.css";

                ReturnUrl = "https://localhost:44373/fawry/ChargeResponse";

                StatusUrl = "https://atfawry.fawrystaging.com/ECommerceWeb/Fawry/payments/status/v2";
            }
        }

        public ChargeRequest BuildChargeRequest(PayRequest model)
        {
            ChargeRequest chargeRequest = new()
            {
                ScriptUrl = ScriptUrl,
                StylesheetUrl = StylesheetUrl,
                MerchantCode = MerchantCode,
                MerchantRefNum = model.MerchantRefNum,
                CustomerMobile = model.CustomerMobile,
                CustomerEmail = model.CustomerEmail,
                CustomerName = model.CustomerName,
                PaymentExpiry = DateTime.UtcNow.AddHours(1).Millisecond.ToString(),
                CustomerProfileId = model.CustomerProfileId,
                ReturnUrl = ReturnUrl,
                ChargeItem = new ChargeItem
                {
                    ItemId = model.ChargeItem.ItemId,
                    Description = model.ChargeItem.Description,
                    ImageUrl = model.ChargeItem.ImageUrl,
                    Quantity = model.ChargeItem.Quantity,
                    Price = Math.Round(model.ChargeItem.Price, 2)
                }
            };

            chargeRequest.Signature = ComputeStringToSha256Hash($"{chargeRequest.MerchantCode}" +
                                                                $"{chargeRequest.MerchantRefNum}" +
                                                                $"{chargeRequest.CustomerProfileId }" +
                                                                $"{chargeRequest.ReturnUrl }" +
                                                                $"{chargeRequest.ChargeItem.ItemId}" +
                                                                $"{chargeRequest.ChargeItem.Price}" +
                                                                $"{SecureKey}");

            return chargeRequest;
        }


        public ChargeResponse GetPaymentStatus(string merchantRefNumber)
        {
            ChargeResponse chargeResponse = null;

            try
            {
                string signature = ComputeStringToSha256Hash($"{MerchantCode}" +
                                                             $"{merchantRefNumber}" +
                                                             $"{SecureKey}");
                string url = StatusUrl + $"?merchantCode={MerchantCode}&merchantRefNumber={merchantRefNumber}&signature={signature}";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream newStream = httpWebResponse.GetResponseStream();
                    StreamReader sr = new(newStream);
                    string result = sr.ReadToEnd();
                    chargeResponse = JsonConvert.DeserializeObject<ChargeResponse>(result);
                }
            }
            catch (Exception)
            {
            }
            return chargeResponse;
        }

        public static string ComputeStringToSha256Hash(string plainText)
        {
            // Create a SHA256 hash from string   
            using SHA256 sha256Hash = SHA256.Create();
            // Computing Hash - returns here byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

            // now convert byte array to a string   
            StringBuilder stringbuilder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                stringbuilder.Append(bytes[i].ToString("x2"));
            }
            return stringbuilder.ToString();
        }
    }

    public class fawrypay_request
    {
        public string merchantCode { get; set; }
        public string merchantRefNumber { get; set; }
        public string signature { get; set; }
    }

    public class PayRequest
    {
        public string MerchantRefNum { get; set; }

        public string CustomerMobile { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerName { get; set; }

        public string CustomerProfileId { get; set; }

        public ChargeItem ChargeItem { get; set; }
    }

    public class ChargeRequest
    {
        public string ScriptUrl { get; set; }

        public string StylesheetUrl { get; set; }

        public string MerchantCode { get; set; }

        public string MerchantRefNum { get; set; }

        public string CustomerMobile { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerName { get; set; }

        public string PaymentExpiry { get; set; }

        public string CustomerProfileId { get; set; }

        public ChargeItem ChargeItem { get; set; }

        public string ReturnUrl { get; set; }

        public string Signature { get; set; }
    }

    public class ChargeItem
    {
        public string ItemId { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; }
    }

    public class ChargeResponse
    {
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
    }
}
