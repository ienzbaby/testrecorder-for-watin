using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace XD.Study
{
    /// <summary>
    /// Session接口类
    /// </summary>
    public class SessionFactory
    {
        private SessionFactory(){}

        private static ISessionFactory factory;
        static readonly object padlock = new object();

        public static ISession OpenSession()
        {
            if (factory == null)
            {
                lock (padlock)
                {
                    if (factory == null)
                    {
                        Configuration cfg = new Configuration().Configure();
                        factory = cfg.BuildSessionFactory();
                    }
                }
            }
            return factory.OpenSession();
        }
    }
}
