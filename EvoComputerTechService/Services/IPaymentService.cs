using EvoComputerTechService.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Services
{
    public interface IPaymentService
    {
        public InstallmentModel CheckInstallments(string binNumber, decimal price);
        public PaymentResponseModel Pay(PaymentModel model);
    }
}
