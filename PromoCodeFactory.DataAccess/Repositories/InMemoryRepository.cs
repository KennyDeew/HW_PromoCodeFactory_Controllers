using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> CreateAsync(T entity)
        {
            Data = Data.Concat(new[] { entity });
            return Task.FromResult(entity);
        }

        public void Update(T entity)
        {
            var dataList = Data.ToList();
            var indexbyId = dataList.FindIndex(t => t.Id == entity.Id);
            if (indexbyId == -1)
            {
                dataList[indexbyId] = entity;
            }
            Data = dataList.AsEnumerable();
        }

        public void Delete(Guid id)
        {
            Data = Data.Where(T => T.Id != id);
        }
    }
}