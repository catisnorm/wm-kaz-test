using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WmKazTest.Core.Model;
using WmKazTest.Core.Services;
using WmKazTest.Model;

namespace WmKazTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ObservationController : ControllerBase
    {
        private readonly ObservationService _observationService;

        public ObservationController(ObservationService observationService)
        {
            _observationService = observationService;
        }

        public string[] Get()
        {
            return new[]
            {
                "1110111",
                "0010010",
                "1011101",
                "1011011",
                "0111010",
                "1101011",
                "1101111",
                "1010010",
                "1111111",
                "1111011"
            };
        }

        [HttpPost("add")]
        public async Task<RequestResult> Add([FromBody] AddObservationViewModel model)
        {
            try
            {
                var response = await _observationService.AddObservation(new Observation
                {
                    SequenceId = model.Sequence,
                    Color = model.Observation.Color,
                    Numbers = model.Observation.Numbers
                });
                return new SuccessResult
                {
                    Status = "ok",
                    Response = new
                    {
                        response.Start,
                        response.Missing
                    }
                };
            }
            catch (Exception e)
            {
                return new ErrorResult
                {
                    Status = "error",
                    Msg = e.Message
                };
            }
        }
    }
}