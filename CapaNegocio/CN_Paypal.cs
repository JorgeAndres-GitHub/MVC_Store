using CapaDatos;
using CapaEntidad.Paypal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Paypal
    {
        private readonly PaypalSettings _settings;

        public CN_Paypal(PaypalSettings settings)
        {
            _settings = settings;
        }

        public async Task<Response_Paypal<Response_Checkout>> CrearSolicitud(Checkout_Order order)
        {
            Response_Paypal<Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_settings.UrlPaypal);
                var authToken = Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.SecretId}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var json = JsonConvert.SerializeObject(order);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/v2/checkout/orders", data);
                response_paypal.Status = response.IsSuccessStatusCode;
                if (response_paypal.Status)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    Response_Checkout checkout = JsonConvert.DeserializeObject<Response_Checkout>(jsonRespuesta);
                    response_paypal.Response = checkout;
                }                
            }
            
            return response_paypal;
        }
        public async Task<Response_Paypal<Response_Capture>> AprobarPago(string token)
        {
            Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_settings.UrlPaypal);
                var authToken = Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.SecretId}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var data = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"/v2/checkout/orders/{token}/capture", data);
                response_paypal.Status = response.IsSuccessStatusCode;
                if (response_paypal.Status)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    Response_Capture capture = JsonConvert.DeserializeObject<Response_Capture>(jsonRespuesta);
                    response_paypal.Response = capture;
                }
            }

            return response_paypal;
        }
    }
}
