using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.V2;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Notify;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentAPI.Helpers;
using PaymentAPI.Helpers.Redis;
using PaymentAPI.Models.Dto;
using PaymentAPI.Tools;
using RestSharp;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PaymentAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "pay")]
    [Route("v1/api/[Controller]/[action]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IWeChatPayClient _clientWebChat;
        private readonly IWeChatPayNotifyClient _clientNofityWebChat;
        private readonly IOptions<WeChatPayOptions> _optionsWebChatAccessor;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRedisCacheManager _redisClient;
        private RestClient _client;

        public PaymentController(IWeChatPayClient clientWebChat, IWeChatPayNotifyClient clientNofityWebChat, IOptions<WeChatPayOptions> optionsWebChatAccessor, ILogger<PaymentController> logger, IHttpContextAccessor accessor, IRedisCacheManager redisClient)
        {
            _clientWebChat = clientWebChat;
            _clientNofityWebChat = clientNofityWebChat;
            _optionsWebChatAccessor = optionsWebChatAccessor;
            _accessor = accessor;
            _logger = logger;
            _client= new RestClient(Appsettings.app(new string[] { "BaseAPIUrl" }));
            _redisClient = redisClient;
        }

        [HttpPost]
        public async Task<FileStreamResult> Pay_By_WebChat(int amount,string content)
        {
            ClaimEntity user = TokenHelp.GetUserInfo(HttpContext.Request.Headers["Authorization"]);
            DateTime dt = DateTime.Now;
            var request = new WeChatPayUnifiedOrderRequest
            {
                Body = content,
                OutTradeNo = "5363471-" + dt.ToString("yyyyMMddHHmmssfff"),
                TotalFee = amount,
                NotifyUrl = $"{Appsettings.app(new string[] { "WeChatPayNotifyUrl" })}/v1/api/payment/post_notify_by_webchat",
                TradeType = "NATIVE",
                TimeExpire= dt.AddMinutes(10).ToString("yyyyMMddHHmmss")
            };
            var response = await _clientWebChat.ExecuteAsync(request, _optionsWebChatAccessor.Value);
            var bitmap = QRCoderHelper.GetPTQRCode(response?.CodeUrl, 5);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            string token = _accessor.HttpContext.Request.Headers["Authorization"];
            await _redisClient.SetAsync(request.OutTradeNo, token,TimeSpan.FromMinutes(10));
            _logger.LogInformation($"用 户 ID:{user.Id},用户名:{user.Name}发起支付,二维码生成成功,商户订单:{request.OutTradeNo}");
            return File(new MemoryStream(ms.GetBuffer()), "image/jpeg", HttpUtility.UrlEncode("pay_pic", Encoding.GetEncoding("UTF-8")));
        }

        [HttpPost]
        [AllowAnonymous]
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
                        //PayModel dto = new PayModel();
                        //RestRequest request = new RestRequest("/MIS/CMS/MemberAction/BuyMemberPower", Method.POST);
                        //string token = _accessor.HttpContext.Request.Headers["Authorization"];
                        //token = token.Replace("Bearer ", "");
                        //dto.serialNo = notify.TransactionId;
                        //dto.serialNo = notify.OutTradeNo;
                        //dto.memberId = AuthHelper.GetClaimFromToken(token).Id;
                        //dto.memberName = dto.ownerName = AuthHelper.GetClaimFromToken(token).Name;
                        //_client.AddDefaultHeader("Authorization", "Bearer " + token);
                        //request.AddJsonBody(dto);
                        //var res = await _client.ExecuteAsync(request);
                       string userToken= await _redisClient.GetValueAsync(notify.OutTradeNo);

                        _logger.LogInformation($"商户订单:{notify.OutTradeNo},支付成功");
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
