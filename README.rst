==============
URI path scanf
==============

.. image:: https://travis-ci.com/DmitryFillo/UriPathScanf.svg?branch=master
     :target: https://travis-ci.com/DmitryFillo/UriPathScanf

TBD: nuget pkg rc, logo, nuget pkg release

Reversed String.Format for URI path.

You can describe URI path like ``/some/path/{id}/some.html`` and parse it to the model like ``{ type: "someUri", id: [id] }``. Supports query string.

.. contents::

Motivation
==========

This package is useful for you if:

* You need to get metadata (identifiers) from URI paths, e.g. to provide API to not to couple some parts of the system with UI specific information.
* You're going to encode data in "URI path format", e.g. ``/{identifierOne}/{identifierTwo}``, and then you're going to decode them to ``IDictionary<string, string>`` or models.

Install
=======

Install `UriPathScanf <https://www.nuget.org/packages/UriPathScanf>`_ via nuget.

Usage
=====

.. code:: c#

  var descriptors = new[]
  {
      new UriPathDescriptor("/path/some/{varOne}", "firstType"),
      new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "secondType"),
  };

  var uriPathScanf = new UriPathScanf(descriptors);

  // scan some URI path
  var result = uriPathScanf.Scan("/path/some/xxx?a=1");
  
  // result.UriType == "firstType"
  // result.Meta is Dictionary<string, string> with "varOne" == "xxx" and "qs__a" = "1" 
  
  // scan some URI path
  result = uriPathScanf.Scan("/path/some/xxx/123y////?a=1");
  
  // result.UriType == "secondType"
  // result.Meta is Dictionary<string, string> with "someVar" == "xxx", "someVar2" == "123y" and "qs__a" = "1" 
  
Typed descriptors:

.. code:: c#

  class Meta : IUriPathMetaModel
  {
      [UriMeta("someVar")]
      public string SomeVar { get; set; }

      [UriMeta("someVar2")]
      public string SomeVar2 { get; set; }

      [UriMetaQuery("x")]
      public string SomeVarQueryString { get; set; }
   }

  var descriptors = new[]
  {
      new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "firstType", typeof(Meta)),
      new UriPathDescriptor("/some/path/{someVar}/{someVar2}", "secondType", typeof(Meta)),
  };

  var uriPathScanf = new UriPathScanf(descriptors);

  var result = uriPathScanf.Scan<Meta>("/path/some/1/2");
  
  // result.UriType == "firstType"
  // result.Meta.SomeVar == 1
  // result.Meta.SomeVar2 == 2
  // result.Meta.SomeVarQueryString = null
  
  result = uriPathScanf.Scan<Meta>("/some/path/1/2/?x=23&b=4");
  
  // result.UriType == "secondType"
  // result.Meta.SomeVar == 1
  // result.Meta.SomeVar2 == 2
  // result.Meta.SomeVarQueryString = "23"
  
Diffrenently typed descriptors:

.. code:: c#

  class Meta : IUriPathMetaModel
  {
      [UriMeta("someVar")]
      public string SomeVar { get; set; }

      [UriMeta("someVar2")]
      public string SomeVar2 { get; set; }
  }
   
  class Meta2 : IUriPathMetaModel
  {
      [UriMetaQuery("x")]
      public string X { get; set; }
  }

  var descriptors = new[]
  {
      new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "firstType", typeof(Meta)),
      new UriPathDescriptor("/some/path/", "secondType", typeof(Meta2)),
  };

  var uriPathScanf = new UriPathScanf(descriptors);

  var result = uriPathScanf.Scan<Meta>("/path/some/1/2");
  
  // result.UriType == "firstType"
  // result.Meta.SomeVar == 1
  // result.Meta.SomeVar2 == 2
  
  result = uriPathScanf.Scan<Meta>("/some/path/1/2/?x=23&b=4");
  
  // result == null
  
  result = uriPathScanf.Scan<Meta>("/some/path/");
  
  // result == null
  
  result = uriPathScanf.Scan<Meta2>("/some/path/");
  
  // result.UriType == "secondType"
  // result.Meta.X == null
  
  result = uriPathScanf.ScanAll("/some/path/x=x");
  
  // result.UriType == "secondType"
  // result.Meta is object
  
  // You can use type pattern matching (is / switch case)
  // e.g. result.Meta is Meta2 
  // or these methods:
  
  result.TryCast<Meta>(out var resultCastedToMeta)
  
  // resultCastedToMeta == null
  
  result.TryCast<Meta2>(out var resultCastedToMeta2) 
  
  // resultCastedToMeta2 is Meta2
  // resultCastedToMeta2.X = "x"
  
Typed and non-typed descriptors:

.. code:: c#

  class Meta : IUriPathMetaModel
  {
      [UriMeta("someVar")]
      public string SomeVar { get; set; }

      [UriMeta("someVar2")]
      public string SomeVar2 { get; set; }
  }

  var descriptors = new[]
  {
      new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "someType", typeof(Meta)),
      new UriPathDescriptor("/some/path/", "someType"),
  };

  var uriPathScanf = new UriPathScanf(descriptors);

  var result = uriPathScanf.ScanAll("/path/some/1/2");
  
  // result.UriType == "someType"
  // result.Meta is object
  // result.Meta.SomeVar2 == 2
    
  // You can use type pattern matching (is / switch case)
  // e.g. result.Meta is Meta
  // or these methods:
  
  result.TryCast<Meta>(out var resultCastedToMeta)
  
  // resultCastedToMeta is Meta
  // resultCastedToMeta.SomeVar = "1"
  // resultCastedToMeta.SomeVar2 = "2"
  
  result.TryCastToDict(out var resultCastedToDict) 
  
  // resultCastedToDict == null
  
  result = uriPathScanf.ScanAll("/some/path/?x=3&m=n");
  
  // result.UriType == "someType"
  // result.Meta is object
  
  result.TryCastToDict(out resultCastedToDict) 
  
  // resultCastedToDict is Dictionary<string, string> with keys "qs__x" and "qs__m"
  
  result = uriPathScanf.Scan<Meta>("/path/some/1/2");
  
  // result.UriType == "someType"
  // result.Meta is Meta
  // result.Meta.SomeVar == 1
  // result.Meta.SomeVar2 = 2
  
  result = uriPathScanf.Scan<Meta>("/some/path/?x=3&m=n");
  
  // result = null


Examples
========

Check examples in the project ``UriPathScanf.Example``.
