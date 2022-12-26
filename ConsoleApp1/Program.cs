using System.Collections.Generic;

using NHibernate;

using ProductLibrary.Model;
using ProductLibrary.Model.NHibernate;

var product = new Product(name:"булка", manufacturerId:Guid.Parse("DEA29528-56F1-4A7F-8B56-7B8181F17631"),price: "25");
var manufacturer = new Manufacturer("ИграМолоко","пос.Игра");
using (ISession session = NHibernateHelper.OpenSession())
using (ITransaction transaction = session.BeginTransaction())
{
  session.Save(product);
  transaction.Commit();
}

