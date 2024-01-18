using System.Collections.Generic;
using System.Linq;

namespace Home.OutBox.Service;

public class OutBox {
    private OutBox() {

    }

    public OutBox(string sender, string type, string message) {
        Sender = sender;
        Type = type;
        Message = message;
        Properties = new List<OutBoxProperty>();
        AddProperty("sender", sender);
        AddProperty("type", type);
    }

    public int Id { get; set; }
    public string Sender { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }

    public List<OutBoxProperty> Properties { get; set; }

    public void AddProperty(string key, string value) {
        Properties.Add(new OutBoxProperty(key, value));
    }

    public IReadOnlyList<KeyValuePair<string, string>> PropertiesToKeyValue() => Properties.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList().AsReadOnly();
}
