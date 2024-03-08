using System.Runtime.Serialization;

namespace Entities.LinkModels;

[DataContract]
public class LinkResourceBase
{
    public LinkResourceBase() { }

    [DataMember]
    public List<Link> Links { get; set; } = [];
}