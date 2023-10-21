using ConverterRestApi;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ConverterRestApi.Controllers
{
    [Route("api/[controller]")]
    public class DataConvertersController : ControllerBase
    {
        private readonly ConveterTools converter;
        public DataConvertersController(ConveterTools _converter)
        {
            converter = _converter;
        }

        [HttpPost]
        public IActionResult Index(Request req)
        {
            Response response = converter.DoConvert(req.Num, req.FromUnit, req.ToUnit);
            if (response.ResCode == 200)
            {
                return Ok(response.ResMsg);
            }
            else
                return BadRequest(response.ResMsg);

        }
    }
}