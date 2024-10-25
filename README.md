# ProjetoL3 - Preencher Endereço por CEP

Este projeto faz o preenchimento automático dos campos de endereço em uma entidade do Microsoft Dynamics CRM, consultando uma API e fornecendo os dados do endereço com base no CEP.

## Funcionalidade

O plugin `PreencherEnderecoPorCEP` captura o valor do campo **address1_postalcode** (CEP) e faz uma chamada para a API do [ViaCEP](https://viacep.com.br) para obter os dados do endereço correspondente. Após a busca bem-sucedida, o plugin preenche automaticamente os seguintes campos da entidade:

- **address1_line1**: Logradouro
- **address1_city**: Cidade
- **address1_stateorprovince**: Estado
- **cr788_endereco1bairro**: Bairro
- **address1_country**: País (definido como "Brasil")

### Requisitos:

- A entidade deve conter o campo **address1_postalcode** com o CEP válido.
- O CEP deve ser composto por 8 dígitos numéricos.

## Configuração


### Pré-requisitos

- Microsoft Dynamics CRM (on-premises ou online)
- API ViaCEP
- .NET Framework e Microsoft.Xrm.Sdk
- Microsoft.CrmSdk.CoreAssemblies
- Newtonsoft.Json
- Microsoft.CrmSdk.XrmTooling.PluginRegistrationTool

### Instalação

1. Clone o repositório do projeto:
https://github.com/Emerson143/ProjetoL3/
