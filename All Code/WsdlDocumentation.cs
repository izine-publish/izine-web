using System;

using System.CodeDom;

using System.Collections.Generic;

using System.Collections.ObjectModel;

using System.ServiceModel;

using System.ServiceModel.Description;

using System.Web.Services.Description;

using System.Xml;

using System.Xml.Schema;
using System.ServiceModel.Activation;


namespace IndiDocumentation
{

    public class WsdlDocumentationAttribute : Attribute,

    IContractBehavior,

    IOperationBehavior,

    IWsdlExportExtension,

    IServiceContractGenerationExtension,

    IOperationContractGenerationExtension
    {

        const int MaxCommentLineLength = 80;

        ContractDescription contractDescription;

        OperationDescription operationDescription;

        string text;

        public WsdlDocumentationAttribute(string text)
        {

            this.text = text;

        }

        public string Text
        {

            get { return this.text; }

            set { this.text = value; }

        }

        #region WSDL Export

        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {

            if (contractDescription != null)
            {

                context.WsdlPortType.Documentation = this.Text;

            }

            else
            {

                Operation operation = context.GetOperation(operationDescription);

                if (operation != null)
                {

                    operation.Documentation = this.Text;

                }

            }

        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context) { }

        #endregion

        #region IContractBehavior Members

        //void IContractBehavior.ApplyDispatchBehavior(ContractDescription description, IEnumerable<ServiceEndpoint> endpoints, System.ServiceModel.Dispatcher.DispatchRuntime dispatch)
        //{

        //    this.contractDescription = description;

        //}

        void IContractBehavior.AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection parameters) { }

        void IContractBehavior.ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime proxy) { }

        void IContractBehavior.Validate(ContractDescription description, ServiceEndpoint endpoint) { }

        #endregion

        #region IOperationBehavior Members

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
        {

            this.operationDescription = description;

        }

        void IOperationBehavior.AddBindingParameters(OperationDescription description, System.ServiceModel.Channels.BindingParameterCollection parameters) { }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy) { }

        void IOperationBehavior.Validate(OperationDescription description) { }

        #endregion

        #region Code Generation

        public void GenerateContract(ServiceContractGenerationContext context)
        {

            context.ContractType.Comments.Add(new CodeCommentStatement("From WSDL PortType Documentation:", true));

            context.ContractType.Comments.Add(new CodeCommentStatement(String.Empty, true));

            foreach (string line in FormatLines(this.Text, WsdlDocumentationAttribute.MaxCommentLineLength))
            {

                context.ContractType.Comments.Add(new CodeCommentStatement(line, true));

            }

            context.ContractType.Comments.Add(new CodeCommentStatement(String.Empty, true));

        }

        public void GenerateOperation(OperationContractGenerationContext context)
        {

            context.SyncMethod.Comments.Add(new CodeCommentStatement("From WSDL Operation Documentation:", true));

            context.SyncMethod.Comments.Add(new CodeCommentStatement(String.Empty, true));

            foreach (string line in FormatLines(this.Text, WsdlDocumentationAttribute.MaxCommentLineLength))
            {

                context.SyncMethod.Comments.Add(new CodeCommentStatement(line, true));

            }

            context.SyncMethod.Comments.Add(new CodeCommentStatement(String.Empty, true));

        }

        Collection<string> FormatLines(string text, int columnWidth)
        {

            Collection<string> lines = new Collection<string>();

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            foreach (string word in text.Split(' '))
            {

                if ((builder.Length > 0) && ((builder.Length + word.Length) > columnWidth))
                {

                    lines.Add(builder.ToString());

                    builder = new System.Text.StringBuilder();

                }

                builder.Append(word);

                builder.Append(' ');

            }

            lines.Add(builder.ToString());

            return lines;

        }

        #endregion


        #region IContractBehavior Members


        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
            this.contractDescription = contractDescription;
        }

        #endregion

        
    }

}