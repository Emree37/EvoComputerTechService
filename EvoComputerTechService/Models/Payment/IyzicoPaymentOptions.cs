using Iyzipay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Models.Payment
{
    public class IyzicoPaymentOptions : Options
    {
        public const string Key = "IyzicoOptions";
        public string ThreedsCallbackUrl { get; set; }
    }
}
