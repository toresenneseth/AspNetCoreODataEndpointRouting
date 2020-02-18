# Dynamic Edm Model creation with ASP.NET Core OData Endpoint routing on ASP.NET Core 3.1,

## About the code

The solution is based on the AspNetCore3xEndpointSample built by the OData team at Microsoft.
The source code from https://github.com/OData/WebApi is included (not using the NuGet, because I needed to step through in an attempt to figure out what was going on)

- Build and run, then go to

```
http://localhost:21504/odata/MySource/Table1
```

("MySource" and "Table1" are hard-coded, so trying other paths will not work)

**Configure** in Startup.cs contains the code for dynamically generating the Edm model and configuring the routing so that the **HandleAllController** handles all OData requests.
When http://localhost:21504/odata/MySource/Table1 is requested, HandleAllController.Get() is called, which in turn resolves the correct package, db table and returns the response.
**SqlDataSource** generates the Edm model and the response data for the request.

The response to the browser is

```
{"@odata.context":"http://localhost:21504/odata/%7BdataSource%7D/$metadata#Table1","value":[
```

because an exception is thrown by the **ResourceSetWithoutExpectedTypeValidator**.

## Some background info...

This is the scenario:
We have a system that (among other things) let db / system admins expose a set of database tables through an OData API, using just configuration.
Through a user interface, they basically define packages which describes which db tables that they want to expose to different type of users.
This means that we (devs) need to dynamically generate the Edm model and response data from the package configuration(s) based on http requests.

My initial attempt to get this working was based on the NuGet nightly build
https://www.myget.org/feed/webapinetcore/package/nuget/Microsoft.AspNetCore.OData/7.4.0-Nightly202001292116

however, I couldn't get it working, so I downloaded the Microsoft.AspNetCore.OData source code to be able to debug properly.
As stated in the github issue, https://github.com/OData/WebApi/issues/2059, my problem is that the serialization is terminated due to some validation that happens in **Microsoft.OData.ResourceSetWithoutExpectedTypeValidator**. We had this working with .Net Framework using the following NuGet packages:

```xml
<package id="Microsoft.AspNet.OData" version="7.0.1" targetFramework="net472" />
<package id="Microsoft.OData.Core" version="7.5.0" targetFramework="net472" />
<package id="Microsoft.OData.Edm" version="7.5.0" targetFramework="net472" />
<package id="Microsoft.Spatial" version="7.5.0" targetFramework="net472" />
```

and it seems like there was introduced a change to how **ResourceSetWithoutExpectedTypeValidator** did validation in June 2019 (merge #1463). It seems strange that this change is what's causing this issue, so it's probably caused by something else. Just thought I should mention it.
