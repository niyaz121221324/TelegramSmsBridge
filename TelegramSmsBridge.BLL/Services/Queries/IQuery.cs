using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramSmsBridge.BLL.Services.Queries;

public interface IQuery<T> where T : class
{
    Task<T?> GetData();
}