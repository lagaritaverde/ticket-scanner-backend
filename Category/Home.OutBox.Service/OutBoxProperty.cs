namespace Home.OutBox.Service;

public class OutBoxProperty {
    public OutBoxProperty() {

    }

    public OutBoxProperty(string key, string value) {
        Key = key;
        Value = value;
    }

    public string Key { get; set; }
    public string Value { get; set; }
}
