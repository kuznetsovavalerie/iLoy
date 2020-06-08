using System.Threading;
using System.Threading.Tasks;

namespace ILoyInterview.Domain.Abstract
{
    public interface ICrudManager<TModel>
    {
        public Task<int> CreateAsync(TModel model, CancellationToken token);

        public Task UpdateAsync(int id, TModel model, CancellationToken token);

        public Task DeleteAsync(int id, CancellationToken token);

        public Task<TModel> GetAsync(int id, CancellationToken token);
    }
}
