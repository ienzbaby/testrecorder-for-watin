using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibernate;
using System.Collections;

namespace XD.Study
{
    [TestFixture]
    public class Test_Actor
    {
        public void Add()
        {
            ISession session = SessionFactory.OpenSession();
            Actor actor = new Actor();
            actor.Id = 1;
            actor.Name = "hao";
            actor.Password = "123";
            session.Save(actor);
            session.Flush();
        }
        public void GetModel()
        {
            ISession session = SessionFactory.OpenSession();
            Actor actor = session.Get<Actor>(1);

            Assert.IsNotNull(actor);

            Console.WriteLine(actor.Name);
            Console.WriteLine(actor.Password);
        }

        public void Delete()
        {
            ISession session = SessionFactory.OpenSession();
            Actor actor = session.Get<Actor>(1);

            session.Delete(actor);
            session.Flush();
        }


        public void GetList()
        {
            ISession session = SessionFactory.OpenSession();
            IList<Actor> list = session.CreateQuery("from Actor").List<Actor>();
            Console.WriteLine(list.Count);
        }
    }
}
