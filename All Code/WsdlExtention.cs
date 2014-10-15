using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;
namespace IndiDocumentation
{
    public class WsdlDocumentationImportExtension : IWsdlImportExtension
    {
        #region WSDL Import
        public void ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
            // Contract Documentation
            if (context.WsdlPortType.Documentation != null)
            {
                context.Contract.Behaviors.Add(new WsdlDocumentationAttribute(context.WsdlPortType.Documentation));
            }
            // Operation Documentation
            foreach (Operation operation in context.WsdlPortType.Operations)
            {
                if (operation.Documentation != null)
                {
                    OperationDescription operationDescription = context.Contract.Operations.Find(operation.Name);
                    if (operationDescription != null)
                    {
                        operationDescription.Behaviors.Add(new WsdlDocumentationAttribute(operation.Documentation));
                    }
                }
            }
        }
        public void BeforeImport(ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy) { }
        public void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context) { }
        #endregion
    }
}