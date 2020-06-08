using ILoyInterview.Contracts.ApiModels.Common;
using ILoyInterview.Domain.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ILoyInterview.API.Controllers.Base
{
    public class ApiBaseController<TManager, TModel> : ControllerBase where TManager : ICrudManager<TModel>
    {
        private readonly TManager _manager;

        public ApiBaseController(TManager manager)
        {
            _manager = manager;
        }

        // Authoriation should be here, but as it's test project, wont spend time on it
        [HttpPost("[controller]")]
        public async Task<IActionResult> Post(TModel model)
        {
            return await WrapResponseAsync(token =>
            {
                return _manager.CreateAsync(model, token);
            });
        }

        [HttpPut("[controller]/{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, TModel model)
        {
            return await WrapResponseAsync(token =>
            {
                return _manager.UpdateAsync(id, model, token);
            });
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            return await WrapResponseAsync(token =>
            {
                return _manager.GetAsync(id, token);
            });
        }

        [HttpDelete("[controller]/{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            return await WrapResponseAsync(async token =>
            {
                await _manager.DeleteAsync(id, token);
            });
        }

        protected async Task<IActionResult> WrapResponseAsync<T>(Func<CancellationToken, Task<T>> responseBuilder)
        {
            var token = this.HttpContext.RequestAborted;
            var response = new ResponseWrapperModel<T>();

            try
            {
                response.Data = await responseBuilder(token);
                response.Success = true;
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Success = false;
            }

            var json = ToJson(response);

            return Content(json, "application/json");
        }

        protected async Task<IActionResult> WrapResponseAsync(Func<CancellationToken, Task> responseBuilder)
        {
            var token = this.HttpContext.RequestAborted;
            var response = new ResponseWrapperModel<object>();

            try
            {
                await responseBuilder(token);
                response.Success = true;
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Success = false;
            }

            var json = ToJson(response);

            return Content(json, "application/json");
        }

        protected string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}
