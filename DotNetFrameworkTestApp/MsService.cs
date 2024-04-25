using Microsoft.Web.Services3.Security.Tokens;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace DotNetFrameworkTestApp
{
    internal class Program
    {
        public static MRSWebServices CreateServiceObject(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            MRSWebServices mcService = new MRSWebServices { Url = url };
            mcService.RequestSoapContext.Addressing.To = new Microsoft.Web.Services3.Addressing.To(new Uri(ServiceConstants.SERVICE_URL));
            mcService.RequestSoapContext.Addressing.ReplyTo = new Microsoft.Web.Services3.Addressing.ReplyTo(new Uri(ServiceConstants.SERVICE_URL));
            mcService.RequestSoapContext.Addressing.Action = ServiceConstants.BASE_URL + ServiceConstants.Action.getCustomerVerificationCs;

            //Setting the policy from the wse3policyCache.config file(make sure there is an app.config and wse3policyCache.config files)
            mcService.SetPolicy(ClientConstants.CLIENT_POLICY);

            //get the locations of the certificates

            X509Certificate2 x509CertificateSign = CreateLoadCert(CertBase64.SignCert, "apphosting");// LoadCert("sign.pfx", "apphosting");
            X509Certificate2 x509CertificateClient = CreateLoadCert(CertBase64.ClientCert, "apphosting");//LoadCert("client.pfx", "apphosting");
            X509Certificate2 x509CertificatePublicKey = CreateLoadCert(CertBase64.PublicCert);//LoadCert("public.cer");

            //encryption certificate
            mcService.ClientCertificates.Add(x509CertificateSign);

            //signing certificate
            mcService.SetClientCredential(new X509SecurityToken(x509CertificateClient));

            //public key
            mcService.SetServiceCredential(new X509SecurityToken(x509CertificatePublicKey));

            return mcService;
        }


        private static X509Certificate2 CreateLoadCert(string base64Content, string pwd = null)
        {
            X509Certificate2 x509CertificateSign = new X509Certificate2(Convert.FromBase64String(base64Content), pwd);
            return x509CertificateSign;
        }

        private static X509Certificate2 LoadCert(string name, string pwd = null)
        {
            //string certDir = Path.Combine(Path.GetDirectoryName("C:\\Playground\\DotNetFrameworkTestApp\\DotNetFrameworkTestApp\\bin\\a.png"), "Certs");
            string certDir = Path.Combine(Path.GetDirectoryName("C:\\playground\\dotnetframeworktestapp\\dotnetframeworktestapp\\bin\\Certs\\a.png"), "Certs");
            X509Certificate2 x509CertificateSign = new X509Certificate2(System.IO.File.ReadAllBytes(Path.Combine(certDir, name)), pwd);
            return x509CertificateSign;
        }

        private static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var webService = default(MRSWebServices);
            CustomerVerificationCsResp response = new CustomerVerificationCsResp();
            MasterCardGetAccountRequest request = new MasterCardGetAccountRequest()
            {
                SourceId = 4,
                Language = "en_US",
                VirtualId = "00453776230049568631"
            };
            // var sorUrl = "https://mtf.ws.loyaltygateway.com/rewards/201010/MRSWebService/Service.asmx";
            var sorUrl = "https://mtf.ws.loyaltygateway.com/rewards/201206/MRSWebService/Service.asmx";
            // var sorUrl = "https://sg-xmlgw.orxenterprise.com/MasterCard/Service.asmx";
            //here we will catch any possible exceptions while setting up the services and certs
            try
            {
                //create the service, get certs and keys
                webService = CreateServiceObject(sorUrl);
                response = webService.getCustomerVerificationCs(request.SourceId, request.BankAccountNumber, request.VirtualId, request.ClientVirtualId, request.Language);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Console.WriteLine("<<<<<<<<<<<======>>>>>>>>>>>");
        }

        public static class ClientConstants
        {
            public const string CLASS_NAME = "ClientMasterCard";
            public const string CLIENT_POLICY = "ClientPolicy";
            public const int CLIENTID = 32;
            public const string LANGUAGE = "en_US";
        }

        public static class ServiceConstants
        {
            public const string BASE_URL = "https://ws.mcrewards.com/";
            public const string SERVICE_URL = "https://ws.mcrewards.com/MRSWebService/Service.asmx";

            public enum Action
            {
                getCustomerVerificationCs = 1,
                doTravelRedemption = 2,
                doCredit = 3,
            }
        }

        public class MasterCardGetAccountRequest
        {
            public ServiceConstants.Action Action { get; set; }
            public string BankAccountNumber { get; set; }
            public string ClientVirtualId { get; set; }
            public string Language { get; set; }
            public long SourceId { get; set; }
            public string VirtualId { get; set; }
        }
    }
}