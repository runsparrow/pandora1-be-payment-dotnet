using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentAPI.Models.Dto
{
    public class PayModel
    {
        public int id { get; set; }
        public string serialNo { get; set; } = "";
        public string orderNo { get; set; } = "";
        public int payerId { get; set; } = -1;
        public string payerName { get; set; } = "";
        public string payerRealName { get; set; } = "";
        public string payerAccount { get; set; } = "";
        public int receiverId { get; set; } =-1;
        public string receiverName { get; set; } = "";
        public string receiverRealName { get; set; } = "";
        public string receiverAccount { get; set; } = "";
        public string dealDateTime { get; set; }
        public decimal dealAmount { get; set; } = 0;
        public string dealType { get; set; }
        public int paySourceIndex { get; set; } = 0;
        public string remark  { get;set; }="";
        public string createDateTime { get; set; }
        public int createUserId { get; set; } = -1;
        public string editDateTime { get; set; } = "";
        public int editUserId { get; set; } = -1;
        public int statusId { get; set; } = -1;
        public string statusName { get; set; } = "";
        public int statusValue { get; set; } = 0;
        public int memberPowerId { get; set; } = 0;
        public string memberPowerName { get; set; } = "";
    }
}
