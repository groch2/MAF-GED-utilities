<Query Kind="Statements">
  <Reference Relative="..\..\..\source\repos\WEB_service_GED_test_client\bin\Debug\net6.0\WEB_service_GED_test_client.dll">C:\Users\deschaseauxr\source\repos\WEB_service_GED_test_client\bin\Debug\net6.0\WEB_service_GED_test_client.dll</Reference>
</Query>

typeof(ServiceReference1.GEDServiceClient).GetMethods()
	.First(m => m.Name == "UploadDocument")
	.GetParameters()
	.Select(p => new { p.Name, DefaultValue = GetDefault(p.ParameterType) })
	.Select(p => $"{p.Name}: {p.DefaultValue ?? "null"},")
	.Dump();

static object GetDefault(Type type) {
   if (type.IsValueType) {
      return Activator.CreateInstance(type);
   }
   return null;
}