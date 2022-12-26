using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ProductLibrary.Model.NHibernate
{
  public class NHibernateHelper

  {
    private static ISessionFactory _sessionFactory;
    private static ISessionFactory SessionFactory
    {
      get
      {
        if (_sessionFactory == null)
        {
          var configuration = new Configuration();
          configuration.Configure();
          configuration.AddAssembly(typeof(Product).Assembly);
          _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
      }
    }
    public static ISession OpenSession()
    {
      return SessionFactory.OpenSession();
    }
  }
}
