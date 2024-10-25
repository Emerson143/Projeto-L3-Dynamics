using System;
using System.ServiceModel;
using System.Net.Http;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;

namespace ProjetoL3
{
    public class PreencherEnderecoPorCEP : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtém os serviços necessários
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Verifica se o contexto possui a entidade alvo
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entidade = (Entity)context.InputParameters["Target"];

                    // Verifica se o atributo do CEP está presente
                    if (entidade.Attributes.Contains("address1_postalcode"))
                    {
                        string cep = entidade["address1_postalcode"].ToString();

                        // Remove caracteres não numéricos do CEP
                        cep = System.Text.RegularExpressions.Regex.Replace(cep, "[^0-9]", "");

                        // Verifica se o CEP possui 8 dígitos
                        if (cep.Length == 8)
                        {
                            // Chama a API para obter os dados do endereço
                            string url = $"https://viacep.com.br/ws/{cep}/json/";

                            using (HttpClient client = new HttpClient())
                            {
                                var response = client.GetAsync(url).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    string json = response.Content.ReadAsStringAsync().Result;
                                    JObject endereco = JObject.Parse(json);

                                    // Verifica se o CEP foi encontrado
                                    if (endereco["erro"] == null)
                                    {
                                        // Preenche os campos do endereço na entidade
                                        entidade["address1_line1"] = (string)endereco["logradouro"];
                                        entidade["address1_city"] = (string)endereco["localidade"];
                                        entidade["address1_stateorprovince"] = (string)endereco["uf"];
                                        entidade["cr788_endereco1bairro"] = (string)endereco["bairro"];
                                        entidade["address1_country"] = "Brasil";

                                    }
                                    else
                                    {
                                        tracingService.Trace("CEP não encontrado.");
                                    }
                                }
                                else
                                {
                                    tracingService.Trace("Falha ao chamar a API do CEP.");
                                }
                            }
                        }
                        else
                        {
                            tracingService.Trace("CEP inválido.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("Erro: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Ocorreu um erro ao preencher o endereço pelo CEP.", ex);
            }
        }
    }
}
