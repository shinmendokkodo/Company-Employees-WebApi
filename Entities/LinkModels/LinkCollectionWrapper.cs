using System.Runtime.Serialization;

namespace Entities.LinkModels;

[DataContract(Name = "LinkCollectionWrapper")]
public class LinkCollectionWrapper<T> : LinkResourceBase
{
    [DataMember(Name = "Value")]
    public List<T> Value { get; set; } = [];

    public LinkCollectionWrapper() { }

    public LinkCollectionWrapper(List<T> value) => Value = value;

}