using NHibernate;
using ProductLibrary.Model.NHibernate;
using ProductLibrary.Model;

namespace SQLService
{
  public static class SQLService<T>
  {  

    public static void Save(T item)
    {
      using (ISession session = NHibernateHelper.OpenSession())
      using (ITransaction transaction = session.BeginTransaction())
      {
        session.Save(item);
        transaction.Commit();
      }
    }

    public static T GetById(Guid id)
    {
      using (ISession session = NHibernateHelper.OpenSession())
        return session.Get<T>(id);
    }

    public static List<T> GetAll()
    {
      using (ISession session = NHibernateHelper.OpenSession())
        return new List<T>(session.CreateCriteria(typeof(T)).List<T>());
    }
    public static List<T> GetAllExpanded()
    {
      
      using (ISession session = NHibernateHelper.OpenSession())        
        return new List<T>(session.CreateCriteria(typeof(T)).List<T>());
      
    }

    public static void Delete(T item)
    {
      using (ISession session = NHibernateHelper.OpenSession())
      using (ITransaction transaction = session.BeginTransaction())
      {
        session.Delete(item);
        transaction.Commit();
      }
    }   
    
    public static void DeleteRange(List<T> list)
    {
      using (ISession session = NHibernateHelper.OpenSession())
      using (ITransaction transaction = session.BeginTransaction())
      {
        foreach (var item in list) session.Delete(item);
        transaction.Commit();
      }
    }

    public static void SaveRange(List<T> list)
    {
      using (ISession session = NHibernateHelper.OpenSession())
      using (ITransaction transaction = session.BeginTransaction())
      {
        foreach (var item in list) session.Save(item);
        transaction.Commit();
      }

    }
  }
}