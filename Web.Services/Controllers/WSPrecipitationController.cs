using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders.Providers;
using Web.Services.Models;

namespace Web.Services.Controllers
{

    //public class PrecipitationModelBinder : IModelBinder
    //{
    //    readonly JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    //    public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
    //    {

    //        Precipitation.Precipitation precip = new Precipitation.Precipitation();
    //        bindingContext.Model = precip;

    //        var content = actionContext.Request.Content;
    //        string json = content.ReadAsStringAsync().Result;
    //        var obj = JsonConvert.DeserializeObject(json, bindingContext.ModelType, settings);
    //        bindingContext.Model = obj;
    //        return true;

            //var targetObject = ServiceLocator.Current.GetInstance(bindingContext.ModelType);
            //var valueProvider = GlobalConfiguration.Configuration.Services.GetValueProviderFactories().First(item => item is QueryStringValueProviderFactory).GetValueProvider(actionContext);

            //foreach (var property in targetObject.GetType().GetProperties())
            //{
            //    var valueAsString = valueProvider.GetValue(property.Name);
            //    var value = valueAsString == null ? null : valueAsString.ConvertTo(property.PropertyType);

            //    if (value == null)
            //        continue;

            //    property.SetValue(targetObject, value, null);
            //}

            //bindingContext.Model = targetObject;
            //return true;
    //    }
    //}


    public class WSPrecipitationController : ApiController
    {

        [HttpPost]
        [Route("api/precipitation/")]
        public ITimeSeries POST(Precipitation.Precipitation precipInput)
        {
            Utilities.Validators utils = new Utilities.Validators();                // Validator
            Utilities.ErrorOutput error = new Utilities.ErrorOutput();              // Used for output generic ITimeSeries object with error message.
            string errorMsg = "";
            if (!utils.ParameterValidation(out errorMsg, precipInput.Input)){
                return error.ReturnError(errorMsg);
            }
            WSPrecipitation precip = new WSPrecipitation();
            ITimeSeries results = precip.GetPrecipitation(out errorMsg, precipInput.Input);
            return results;
        }


    }
}
