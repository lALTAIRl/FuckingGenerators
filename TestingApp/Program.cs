NhibernateAdapter.NhibernateGeneratedAdapter.SetupNhibernate();
NhibernateAdapter.NhibernateGeneratedAdapter.CreateDatabaseSchema();
if (NhibernateAdapter.NhibernateGeneratedAdapter.ValidateSchema())
{
    NhibernateAdapter.NhibernateGeneratedAdapter.PopulateTestData();

    NhibernateAdapter.NhibernateGeneratedAdapter.Test();
}

