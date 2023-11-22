using HiredWorkerManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiredWorkerManagement.Repositories.Interfaces
{
    internal interface IGenericRepo<W> where W : EntityBase
    {
        IEnumerable<W> GetAll(string include = "");
        W Get(int id, string include ="");
        void Insert(W item);
        void Update(W item);
        void Delete(int id);
        K ExcuteSqlSingle<K>(string sql) where K : EntityBase ;
        IEnumerable<K> ExecuteSqlCollection<K>(string sql) where K: EntityBase;
        void ExecuteCommand(string sql);

    }
}
