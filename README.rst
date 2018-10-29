==============
URI path scanf
==============

.. image:: https://travis-ci.com/DmitryFillo/UriPathScanf.svg?branch=master
     :target: https://travis-ci.com/DmitryFillo/UriPathScanf

Reversed String.Format for URI path.

You can describe URI path like ``/some/path/{id}/some.html`` and parse it to the model like ``{ type: "someUri", id: [id] }``. Supports query string.

.. contents::

Motivation
==========

This package is useful for you if:

* If you need to get metadata (identifiers) from URI paths, e.g. to provide API to not to couple some parts of the system with UI specific information.
* If you're going to encode data in "URI path format", e.g. ``/{identifierOne}/{identifierTwo}``, and then you're going to decode them to ``IDictionary<string, string>`` or models.

How to use
==========

Install `UriPathScanf <https://www.nuget.org/packages/UriPathScanf>`_ via nuget.

TBD

Examples
========

Check examples in the project ``UriPathScanf.Example``.
