using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync();

        Task AddAsync(T entity);

        void Update(T entity);

        void SoftDelete(T entity);

        Task SaveAsync();
    }
}
