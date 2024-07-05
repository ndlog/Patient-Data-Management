using Newtonsoft.Json.Linq;
using QLBENHNHAN.Models.Validations;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QLBENHNHAN.Controllers
{
    public class PaymentController : Controller
    {
        private readonly string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
        private readonly string partnerCode = "MOMO";
        private readonly string accessKey = "F8BBA842ECF85";
        private readonly string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

        public async Task<ActionResult> Payment()
        {
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string amount = "10000"; // Số tiền thanh toán
            string orderInfo = "Thanh toán đơn hàng #" + orderId;
            string returnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";
            string notifyUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";

            string requestType = "captureMoMoWallet";
            string extraData = ""; // Dữ liệu bổ sung nếu có

            // Tạo chữ ký
            string rawHash = "partnerCode=" + partnerCode +
                             "&accessKey=" + accessKey +
                             "&requestId=" + requestId +
                             "&amount=" + amount +
                             "&orderId=" + orderId +
                             "&orderInfo=" + orderInfo +
                             "&returnUrl=" + returnUrl +
                             "&notifyUrl=" + notifyUrl +
                             "&extraData=" + extraData;

            string signature = signSHA256(rawHash, secretKey);

            var message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyUrl },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }
            };

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(endpoint, new StringContent(message.ToString(), Encoding.UTF8, "application/json"));
                var responseContent = await response.Content.ReadAsStringAsync();

                // Kiểm tra xem responseContent có dữ liệu không
                if (!string.IsNullOrEmpty(responseContent))
                {
                    var jObject = JObject.Parse(responseContent);

                    // Kiểm tra xem có thuộc tính "payUrl" trong jObject không
                    if (jObject.TryGetValue("payUrl", out JToken payUrlToken) && payUrlToken.Type == JTokenType.String)
                    {
                        string payUrl = payUrlToken.Value<string>();
                        return Redirect(payUrl);
                    }
                    else
                    {
                        // Xử lý khi không có payUrl trong response
                        // Ví dụ: Hiển thị lỗi hoặc quay về trang cũ
                        return RedirectToAction("Index", "Home"); // Hoặc trang khác tùy vào logic của bạn
                    }
                }
                else
                {
                    // Xử lý khi responseContent rỗng
                    // Ví dụ: Hiển thị lỗi hoặc quay về trang cũ
                    return RedirectToAction("Index", "Home"); // Hoặc trang khác tùy vào logic của bạn
                }
            }
        }

        private static string signSHA256(string data, string key)
        {
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(data);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }

        public ActionResult Return()
        {
            // Xử lý kết quả sau khi thanh toán
            return View();
        }

        public ActionResult Notify()
        {
            // Xử lý thông báo từ MoMo
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
