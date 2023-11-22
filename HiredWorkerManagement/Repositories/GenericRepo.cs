using HiredWorkerManagement.Models;
using HiredWorkerManagement.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HiredWorkerManagement.Repositories
{
    public class GenericRepo<W> : IGenericRepo<W> where W : EntityBase
    {
        DbContext db;
        DbSet<W> dbSet;
        public GenericRepo(DbContext db)
        {
            this.db = db;
            this.dbSet = db.Set<W>();
        }

        public W Get(int id, string include = "")
        {
            if (include == "")
                return dbSet.FirstOrDefault(X => X.Id == id);
            else dbSet.Include(include).FirstOrDefault();
        }

        public IEnumerable<W> GetAll(string include = "")
        {
            if (include == "")
                return dbSet.ToList();
            else dbSet.Include(include).ToList();
        }

        public void Insert(W item)
        {
            dbSet.Add(item);
            db.SaveChanges();
        }

        public void Update(W item)
        {
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            var item = dbSet.FirstOrDefault(x=> x.id == id);
            if (item != null)
            {
                dbSet.Remove(item);
                db.SaveChanges();
            }
        }
        public void ExecuteCommand(string sql)
        {
            db.Database.ExecuteSqlCommand(sql);
        }

        public IEnumerable<K> ExecuteSqlCollection<K>(string sql) where K : EntityBase
        {
            return db.Database.SqlQuery<K>(sql).ToList();
        }
        public K ExcuteSqlSingle<K>(string sql) where K : EntityBase
        {
            return db.Database.SqlQuery<K>(sql).FirstOrDefault();
        }

            }
}