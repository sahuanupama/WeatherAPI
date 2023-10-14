namespace MongoNotesAPI.Repositories
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(string id);
        void Create(T item);
        void Update(string id, T item);
        void Delete(string id);
    }
}
