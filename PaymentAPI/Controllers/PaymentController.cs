using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.V2;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Notify;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentAPI.Helpers;
using PaymentAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PaymentAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "pay")]
    [Route("v1/api/[Controller]/[action]")]
    //[Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IWeChatPayClient _clientWebChat;
        private readonly IWeChatPayNotifyClient _clientNofityWebChat;
        private readonly IOptions<WeChatPayOptions> _optionsWebChatAccessor;

        public PaymentController(IWeChatPayClient clientWebChat, IWeChatPayNotifyClient clientNofityWebChat, IOptions<WeChatPayOptions> optionsWebChatAccessor, ILogger<PaymentController> logger)
        {
            _clientWebChat = clientWebChat;
            _clientNofityWebChat = clientNofityWebChat;
            _optionsWebChatAccessor = optionsWebChatAccessor;
            _logger = logger;
        }

        [HttpPost]
        public async Task<FileStreamResult> Pay_By_WebChat(int amount,string content)
        {
            var request = new WeChatPayUnifiedOrderRequest
            {
                Body = content,
                OutTradeNo = "5363471-" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                TotalFee = amount,
                NotifyUrl = "http://xs-test.natapp1.cc/v1/api/payment/post_notify_by_webchat",
                TradeType = "NATIVE"
            };
            var response = await _clientWebChat.ExecuteAsync(request, _optionsWebChatAccessor.Value);
            var bitmap = QRCoderHelper.GetPTQRCode(response?.CodeUrl, 5);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            return File(new MemoryStream(ms.GetBuffer()), "image/jpeg", HttpUtility.UrlEncode("pay_pic", Encoding.GetEncoding("UTF-8")));
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Post_Notify_By_Webchat()
        {

            try
            {
                var notify = await _clientNofityWebChat.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request, _optionsWebChatAccessor.Value);
                if (notify.ReturnCode == "SUCCESS")
                {
                    if (notify.ResultCode == "SUCCESS")
                    {
                        Console.WriteLine("OutTradeNo: " + notify.OutTradeNo);
                        return WeChatPayNotifyResult.Success;
                    }
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
        }
    }
}
