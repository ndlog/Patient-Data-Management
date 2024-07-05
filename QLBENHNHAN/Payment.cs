using QLBENHNHAN.Models.Validations;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QLBENHNHAN
{
    public class Payment
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task Main(string[] args)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

            var request = new CollectionLinkRequest
            {
                orderInfo = "pay with MoMo",
                partnerCode = "MOMO",
                redirectUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b",
                ipnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b",
                amount = 5000,
                orderId = myuuidAsString,
                requestId = myuuidAsString,
                requestType = "payWithMethod",
                extraData = "",
                partnerName = "MoMo Payment",
                storeId = "Test Store",
                orderGroupId = "",
                autoCapture = true,
                lang = "vi"
            };

            string rawSignature = $"accessKey={accessKey}&amount={request.amount}&extraData={request.extraData}&ipnUrl={request.ipnUrl}&orderId={request.orderId}&orderInfo={request.orderInfo}&partnerCode={request.partnerCode}&redirectUrl={request.redirectUrl}&requestId={request.requestId}&requestType={request.requestType}";
            request.signature = GetSignature(rawSignature, secretKey);

            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            string contents = await quickPayResponse.Content.ReadAsStringAsync();
            Console.WriteLine(contents);
        }

        private static string GetSignature(string text, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(text));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}