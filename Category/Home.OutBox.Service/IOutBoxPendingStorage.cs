using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Home.OutBox.Service;

public interface IOutBoxPendingStorage {
    Task<OutBox?> Get(CancellationToken cancellationToken = default);
    Task Delete(OutBox outBox, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutBox>> Get(int count, CancellationToken cancellationToken = default);
    Task Delete(IEnumerable<OutBox> outBoxes, CancellationToken cancellationToken = default);
}
