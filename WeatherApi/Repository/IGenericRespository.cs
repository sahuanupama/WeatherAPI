﻿using WeatherApi.Models;

namespace WeatherApi.Repository
    {
    public interface IGenericRespository<T>
        {
        IEnumerable<T> GetAll();
        T GetById(string id);
        void Create(T item);
        OperationResponseDTO<T> Update(string id, T item);
        OperationResponseDTO<T> Delete(string id);
        }
    }
