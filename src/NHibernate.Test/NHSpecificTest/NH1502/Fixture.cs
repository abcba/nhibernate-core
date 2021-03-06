using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1502
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}

		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Person e1 = new Person("Joe", 10, 9);
					Person e2 = new Person("Sally", 100, 8);
					Person e3 = new Person("Tim", 20, 7); //20
					Person e4 = new Person("Fred", 40, 40);
					Person e5 = new Person("Mike", 50, 50);
					s.Save(e1);
					s.Save(e2);
					s.Save(e3);
					s.Save(e4);
					s.Save(e5);
					tx.Commit();
				}
			}
		}

		[Test]
		public void OrderProjectionTest() 
		{
			ISQLFunction arithmaticMultiplication = new VarArgsSQLFunction("(", "*", ")");
			using (ISession session = this.OpenSession()) {
				ICriteria criteria = session.CreateCriteria(typeof (Person), "c");

				criteria.AddOrder(Order.Asc(
				                  	Projections.SqlFunction(arithmaticMultiplication, NHibernateUtil.GuessType(typeof (int)),
				                  	                        Projections.Property("IQ"), Projections.Constant(-1))));
				IList<Person> results=criteria.List<Person>();
				Assert.AreEqual(5, results.Count);
				Assert.AreEqual("Sally", results[0].Name);
				Assert.AreEqual("Joe", results[4].Name);
			}
		}
	}
}