using System.Runtime.Serialization;

namespace Entities.LinkModels;

[DataContract]
public class Link 
{
    [DataMember]
    public string? Href { get; set; }

    [DataMember]
    public string? Rel { get; set; }

    [DataMember]
    public string? Method { get; set; }

    public Link() { }

    public Link(string? href, string? rel, string? method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}