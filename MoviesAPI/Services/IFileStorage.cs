using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public interface IFileStorage
    {
        Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType);
        Task DeleteFile(string path, string container);

        Task<string> SaveFile(byte[] content, string extension, string container, string contentType);
    }
}
