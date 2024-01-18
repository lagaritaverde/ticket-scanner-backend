using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Home.OutBox.Service;

public interface IOutBoxSender {
    Task SendAsync(string id, string type, string data, IReadOnlyList<KeyValuePair<string, string>> customProperties = null, CancellationToken cancellationToken = default);
}
