using System;
using System.Web.Http;
using static DotNetFrameworkTestApp.Program;

namespace DotNetFrameworkTestApp.Controller
{
    public class TestController : ApiController
    {
        // GET: api/Test
        public CustomerVerificationCsResp Get()
        {
            var sorUrl = "https://mtf.ws.loyaltygateway.com/rewards/201206/MRSWebService/Service.asmx";
            MasterCardGetAccountRequest request = new MasterCardGetAccountRequest()
            {
                SourceId = 4,
                Language = "en_US",
                VirtualId = "00453776230049568631"
            };
            var webService = CreateServiceObject(sorUrl);
            try
            {
                webService = CreateServiceObject(sorUrl);

                //create the service, get certs and keys
                var response = webService.getCustomerVerificationCs(request.SourceId, request.BankAccountNumber, request.VirtualId, request.ClientVirtualId, request.Language);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: api/Test/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Test
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Test/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Test/5
        public void Delete(int id)
        {
        }
    }
}
