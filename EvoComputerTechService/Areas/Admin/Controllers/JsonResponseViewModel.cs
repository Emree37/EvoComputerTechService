namespace EvoComputerTechService.Areas.Admin.Controllers
{
    internal class JsonResponseViewModel
    {
        public JsonResponseViewModel()
        {
        }

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}