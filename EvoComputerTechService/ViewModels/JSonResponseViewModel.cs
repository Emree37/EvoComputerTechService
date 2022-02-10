using System;

namespace EvoComputerTechService.ViewModels
{
    public class JSonResponseViewModel
    {
        public bool IsSucces { get; set; }
        public object Data { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;


    }
}
