﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArticlesClient.CommentsService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultDtoOfArrayOfCommentDtoO_Po34nZx", Namespace="http://schemas.datacontract.org/2004/07/Articles.Services.Models")]
    [System.SerializableAttribute()]
    public partial class ResultDtoOfArrayOfCommentDtoO_Po34nZx : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ArticlesClient.CommentsService.CommentDto[] DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SuccessField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ArticlesClient.CommentsService.CommentDto[] Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CommentDto", Namespace="http://schemas.datacontract.org/2004/07/Articles.Services.Models")]
    [System.SerializableAttribute()]
    public partial class CommentDto : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long ArticleIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AuthorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime CreatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long IdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long ArticleId {
            get {
                return this.ArticleIdField;
            }
            set {
                if ((this.ArticleIdField.Equals(value) != true)) {
                    this.ArticleIdField = value;
                    this.RaisePropertyChanged("ArticleId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Author {
            get {
                return this.AuthorField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthorField, value) != true)) {
                    this.AuthorField = value;
                    this.RaisePropertyChanged("Author");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Created {
            get {
                return this.CreatedField;
            }
            set {
                if ((this.CreatedField.Equals(value) != true)) {
                    this.CreatedField = value;
                    this.RaisePropertyChanged("Created");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultDtoOfCommentDtoO_Po34nZx", Namespace="http://schemas.datacontract.org/2004/07/Articles.Services.Models")]
    [System.SerializableAttribute()]
    public partial class ResultDtoOfCommentDtoO_Po34nZx : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ArticlesClient.CommentsService.CommentDto DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SuccessField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ArticlesClient.CommentsService.CommentDto Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CommentsService.ICommentsService")]
    public interface ICommentsService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/GetAll", ReplyAction="http://tempuri.org/ICommentsService/GetAllResponse")]
        ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx GetAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/GetAll", ReplyAction="http://tempuri.org/ICommentsService/GetAllResponse")]
        System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx> GetAllAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Get", ReplyAction="http://tempuri.org/ICommentsService/GetResponse")]
        ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Get(long id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Get", ReplyAction="http://tempuri.org/ICommentsService/GetResponse")]
        System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> GetAsync(long id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/GetForArticle", ReplyAction="http://tempuri.org/ICommentsService/GetForArticleResponse")]
        ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx GetForArticle(long id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/GetForArticle", ReplyAction="http://tempuri.org/ICommentsService/GetForArticleResponse")]
        System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx> GetForArticleAsync(long id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Create", ReplyAction="http://tempuri.org/ICommentsService/CreateResponse")]
        ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Create(ArticlesClient.CommentsService.CommentDto comment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Create", ReplyAction="http://tempuri.org/ICommentsService/CreateResponse")]
        System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> CreateAsync(ArticlesClient.CommentsService.CommentDto comment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Update", ReplyAction="http://tempuri.org/ICommentsService/UpdateResponse")]
        ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Update(ArticlesClient.CommentsService.CommentDto comment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Update", ReplyAction="http://tempuri.org/ICommentsService/UpdateResponse")]
        System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> UpdateAsync(ArticlesClient.CommentsService.CommentDto comment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Delete", ReplyAction="http://tempuri.org/ICommentsService/DeleteResponse")]
        ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Delete(long id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICommentsService/Delete", ReplyAction="http://tempuri.org/ICommentsService/DeleteResponse")]
        System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> DeleteAsync(long id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICommentsServiceChannel : ArticlesClient.CommentsService.ICommentsService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CommentsServiceClient : System.ServiceModel.ClientBase<ArticlesClient.CommentsService.ICommentsService>, ArticlesClient.CommentsService.ICommentsService {
        
        public CommentsServiceClient() {
        }
        
        public CommentsServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CommentsServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CommentsServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CommentsServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx GetAll() {
            return base.Channel.GetAll();
        }
        
        public System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx> GetAllAsync() {
            return base.Channel.GetAllAsync();
        }
        
        public ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Get(long id) {
            return base.Channel.Get(id);
        }
        
        public System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> GetAsync(long id) {
            return base.Channel.GetAsync(id);
        }
        
        public ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx GetForArticle(long id) {
            return base.Channel.GetForArticle(id);
        }
        
        public System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfArrayOfCommentDtoO_Po34nZx> GetForArticleAsync(long id) {
            return base.Channel.GetForArticleAsync(id);
        }
        
        public ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Create(ArticlesClient.CommentsService.CommentDto comment) {
            return base.Channel.Create(comment);
        }
        
        public System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> CreateAsync(ArticlesClient.CommentsService.CommentDto comment) {
            return base.Channel.CreateAsync(comment);
        }
        
        public ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Update(ArticlesClient.CommentsService.CommentDto comment) {
            return base.Channel.Update(comment);
        }
        
        public System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> UpdateAsync(ArticlesClient.CommentsService.CommentDto comment) {
            return base.Channel.UpdateAsync(comment);
        }
        
        public ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx Delete(long id) {
            return base.Channel.Delete(id);
        }
        
        public System.Threading.Tasks.Task<ArticlesClient.CommentsService.ResultDtoOfCommentDtoO_Po34nZx> DeleteAsync(long id) {
            return base.Channel.DeleteAsync(id);
        }
    }
}