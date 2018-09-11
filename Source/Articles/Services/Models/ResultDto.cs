using System;
using System.Runtime.Serialization;

namespace Articles.Services.Models
{
    [DataContract]
    public class ResultDto<T>
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public T Data { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}