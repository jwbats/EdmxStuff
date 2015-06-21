using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using EdmxStuff.Context;

namespace EdmxStuff
{
	public partial class Default : System.Web.UI.Page
	{

		private Person CreatePerson(string firstName, string lastName, int age)
		{
			Person person = new Person()
			{
				FirstName = firstName,
				LastName  = lastName,
				Age       = age
			};

			person.Ragdolls.Add(
				new Ragdoll() {
					Name = "Fleur",
					Color = "Seal point"
				}
			);

			person.Ragdolls.Add(
				new Ragdoll() {
					Name = "Floor",
					Color = "Blue mitted"
				}
			);

			return person;
		}




		private Person GetPerson(int id, bool tracked)
		{
			using (MyDatabaseContext context = new MyDatabaseContext())
			{
				Person person = (tracked)
					? context.Persons.Find(id)
					: context.Persons.AsNoTracking().FirstOrDefault(x => x.Id == id);

				return person;
			}
		}




		private int GetFirstId()
		{
			using (MyDatabaseContext context = new MyDatabaseContext())
			{
				Person person = context.Persons.FirstOrDefault();

				return (person != null) ? person.Id : -1;
			}
		}




		private int AddNewPerson(string firstName, string lastName, int age)
		{
			using (MyDatabaseContext context = new MyDatabaseContext())
			{
				Person person = CreatePerson(firstName, lastName, age);

				context.Persons.Add(person);
				context.SaveChanges();

				return person.Id;
			}
		}




		private void AttachNontrackedPerson(int id)
		{
			using (MyDatabaseContext context = new MyDatabaseContext())
			{
				Person person = GetPerson(id, false);
				person.Age++;

				context.Persons.Attach(person);
				context.Entry(person).State = EntityState.Modified;
				context.SaveChanges();
			}
		}




		private void UpdateTrackedPerson(int id)
		{
			using (MyDatabaseContext context = new MyDatabaseContext())
			{
				Person person = GetPerson(id, true);
				person.Age++;

				context.Entry(person).State = EntityState.Modified;
				context.SaveChanges();
			}
		}




		private void OutputPersonInfo(int id)
		{
			using (MyDatabaseContext context = new MyDatabaseContext())
			{
				Person person1 = context.Persons.FirstOrDefault(x => x.Id == id); // lazy loading off: ragdolls aren't loaded at all
				Person person2 = context.Persons.Include("Ragdolls").FirstOrDefault(x => x.Id == id); // lazy loading off: ragdolls are eager loaded
				
				Debug.WriteLine("{0} has {1} ragdolls.", person2.FirstName, person2.Ragdolls.Count);
				
				foreach (Ragdoll ragdoll in person2.Ragdolls)
				{
					Debug.WriteLine("{0} ({1})", ragdoll.Name, ragdoll.Color);
				}
			}
		}




		protected void Page_Load(object sender, EventArgs e)
		{
			int firstId = GetFirstId();

			int id = (firstId != -1)
				? firstId
				: AddNewPerson("Jay", "Bats", 24);

			AttachNontrackedPerson(id);
			UpdateTrackedPerson(id);

			OutputPersonInfo(id);
		}

	}
}