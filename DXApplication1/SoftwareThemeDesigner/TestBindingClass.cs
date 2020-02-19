namespace SoftwareThemeDesigner
{
	public class TestBindingClass
	{
		public string country { get; set; }

		public TestBindingClass(string country)
		{
			this.country = country;
		}
	}

	public class NestedTestClass
	{
		public TestBindingClass nestedClass { get; set; }

		public NestedTestClass(TestBindingClass nestedClass)
		{
			this.nestedClass = nestedClass;
		}
	}
}