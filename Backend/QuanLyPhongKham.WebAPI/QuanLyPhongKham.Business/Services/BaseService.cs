using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;

        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new ErrorNotFoundException();
            }
            return entity;

        }
        public virtual async Task<int> AddAsync(T entity)
        {
            //try
            //{
            //    return await _repository.AddAsync(entity);
            //}
            //catch (Exception ex)
            //{
            //    throw new ApplicationException("An error occurred while adding the entity.", ex);
            //}
            return -1;
        }

        public virtual async Task<int> UpdateAsync(T entity)
        {
            //try
            //{
            //    return await _repository.UpdateAsync(entity);
            //}
            //catch (Exception ex)
            //{
            //    throw new ApplicationException("An error occurred while updating the entity.", ex);
            //}
            return -1;
        }
        public async Task<int> DeleteAsync(Guid id)
        {
            int result = await _repository.DeleteAsync(id);
            if (result == 0)
            {
                throw new ErrorNotFoundException();
            }
            return result;
        }
    }
}
