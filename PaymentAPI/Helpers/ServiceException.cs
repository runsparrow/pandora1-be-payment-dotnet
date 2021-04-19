using System;

namespace PaymentAPI.Helpers
{
    public class ServiceException : Exception
    {
        public ErrorDescriptor Descriptor { get; set; }
        /// <param name="descriptor"></param>
        public ServiceException(ErrorDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public override string ToString()
        {
            var baseContent = base.ToString();
            return $" errorCode:{Descriptor.errorCode};\r\n errorMessage:{Descriptor.errorMessage};\r\n trackingData:{baseContent}";
        }
    }
}
