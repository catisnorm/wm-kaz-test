using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WmKazTest.Core.Services;
using WmKazTest.Model;

namespace WmKazTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SequenceController : ControllerBase
    {
        private readonly SequenceService _sequenceService;

        public SequenceController(SequenceService sequenceService)
        {
            _sequenceService = sequenceService;
        }

        [HttpPost("create")]
        public async Task<RequestResult> Create()
        {
            try
            {
                var result = await _sequenceService.Create();
                return new SuccessResult
                {
                    Response = new
                    {
                        sequence = result.Id
                    },
                    Status = "ok"
                };
            }
            catch (Exception e)
            {
                return new ErrorResult
                {
                    Msg = e.Message,
                    Status = "error"
                };
            }
        }

        [HttpGet("~/clear")]
        public async Task<RequestResult> Clear()
        {
            try
            {
                await _sequenceService.Clear();
                return new SuccessResult
                {
                    Response = "ok",
                    Status = "ok"
                };
            }
            catch (Exception e)
            {
                return new ErrorResult
                {
                    Msg = e.Message,
                    Status = "error"
                };
            }
        }
    }
}