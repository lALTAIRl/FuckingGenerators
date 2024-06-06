NhibernateAdapter.NhibernateAdapter.SetupNhibernate();
NhibernateAdapter.NhibernateAdapter.CreateDatabaseSchema();
if (NhibernateAdapter.NhibernateAdapter.ValidateSchema())
{
    NhibernateAdapter.NhibernateAdapter.PopulateTestData();

    NhibernateAdapter.NhibernateAdapter.Test();
}

