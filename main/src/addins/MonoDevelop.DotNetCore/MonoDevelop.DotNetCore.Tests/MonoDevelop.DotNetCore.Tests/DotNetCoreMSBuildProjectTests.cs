﻿//
// DotNetCoreMSBuildProjectTests.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2017 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Projects.MSBuild;
using NUnit.Framework;

namespace MonoDevelop.DotNetCore.Tests
{
	[TestFixture]
	class DotNetCoreMSBuildProjectTests
	{
		DotNetCoreMSBuildProject project;
		MSBuildProject msbuildProject;

		void CreateMSBuildProject (string xml, string fileName = @"MyProject.csproj")
		{
			msbuildProject = new MSBuildProject ();
			msbuildProject.FileName = fileName;
			msbuildProject.LoadXml (xml);

			project = new DotNetCoreMSBuildProject ();
		}

		void AddGlobalPropertyToMSBuildProject (string name, string value, string defaultValue = null)
		{
			msbuildProject
				.GetGlobalPropertyGroup ()
				.SetValue (name, value, defaultValue);
		}

		string GetPropertyValueFromMSBuildProject (string name)
		{
			return msbuildProject
				.GetGlobalPropertyGroup ()
				.GetValue (name);
		}

		bool MSBuildProjectHasGlobalProperty (string name)
		{
			return msbuildProject
				.GetGlobalPropertyGroup ()
				.HasProperty (name);
		}

		void ReadProject ()
		{
			project.ReadProjectHeader (msbuildProject);
			project.ReadProject (msbuildProject);
		}

		void WriteProject (string framework = ".NETCoreApp", string version = "1.0")
		{
			var moniker = new TargetFrameworkMoniker (framework, version);
			project.WriteProject (msbuildProject, moniker);
		}

		[Test]
		public void ReadProject_ToolsVersionDefined ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"</Project>");

			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";

			Assert.AreEqual ("15.0", project.ToolsVersion);
			Assert.IsFalse (project.IsOutputTypeDefined);
			Assert.AreEqual ("Microsoft.NET.Sdk", project.Sdk);
			Assert.IsTrue (project.HasSdk);
		}

		[Test]
		public void ReadProject_OutputTypeDefined ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			msbuildProject.Evaluate ();

			ReadProject ();

			Assert.IsTrue (project.IsOutputTypeDefined);
			Assert.AreEqual ("netcoreapp1.0", project.TargetFrameworks.Single ());
		}

		[Test]
		public void WriteProject_ProjectGuidAddedAndToolsVersionChanged_ProjectGuidIsRemovedAndToolsVersionReset ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();

			msbuildProject.ToolsVersion = "4.0";
			AddGlobalPropertyToMSBuildProject ("ProjectGuid", "{111}");
			WriteProject ();

			Assert.AreEqual ("15.0", msbuildProject.ToolsVersion);
			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("ProjectGuid"));
		}

		[Test]
		public void WriteProject_OutputTypeLibraryIsAdded_OutputTypeIsRemoved ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();

			msbuildProject.ToolsVersion = "4.0";
			AddGlobalPropertyToMSBuildProject ("OutputType", "Library");
			WriteProject ();

			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("OutputType"));
		}

		[Test]
		public void WriteProject_OutputTypeLibraryIsDefinedWhenRead_OutputTypeIsNotRemoved ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();

			WriteProject ();

			Assert.IsTrue (MSBuildProjectHasGlobalProperty ("OutputType"));
		}

		[Test]
		public void WriteProject_DefaultTargetsAdded_DefaultTargetsIsSetToNull ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			msbuildProject.DefaultTargets = "Build";

			WriteProject ();

			Assert.IsNull (msbuildProject.DefaultTargets);
		}

		[Test]
		public void AddSdkImports_SdkImportsAdded ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";

			project.AddInternalSdkImports (msbuildProject, "SdkPath", "Sdk.props", "Sdk.targets");

			var firstPropertyGroup = msbuildProject.PropertyGroups.First ();
			Assert.AreEqual ("'$(MSBuildSdksPath)' == ''", firstPropertyGroup.Condition);

			var sdkPathProperty = firstPropertyGroup.GetProperty ("MSBuildSdksPath");
			Assert.AreEqual ("SdkPath", sdkPathProperty.Value);

			Assert.AreEqual ("Sdk.props", msbuildProject.Imports.First ().Project);
			Assert.AreEqual ("Sdk.targets", msbuildProject.Imports.Last ().Project);
		}

		[Test]
		public void WriteProject_SdkImportsAdded_RemovedOnWriting ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";
			project.AddInternalSdkImports (msbuildProject, "SdkPath", "Sdk.props", "Sdk.targets");

			Assert.AreEqual (2, msbuildProject.Imports.Count ());
			Assert.AreEqual (2, msbuildProject.PropertyGroups.Count ());

			WriteProject ();

			Assert.AreEqual (0, msbuildProject.Imports.Count ());
			Assert.AreEqual (1, msbuildProject.PropertyGroups.Count ());

			var firstPropertyGroup = msbuildProject.PropertyGroups.First ();
			Assert.IsFalse (firstPropertyGroup.HasProperty ("MSBuildSdksPath"));
		}

		[Test]
		public void AddSdkImports_CalledTwice_DuplicateImportsNotAdded ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";

			project.AddInternalSdkImports (msbuildProject, "SdkPath", "Sdk.props", "Sdk.targets");
			project.AddInternalSdkImports (msbuildProject, "SdkPath", "Sdk.props", "Sdk.targets");

			Assert.AreEqual (2, msbuildProject.Imports.Count ());
			Assert.AreEqual (2, msbuildProject.PropertyGroups.Count ());
		}

		[Test]
		public void AddSdkImports_MultipleSdkImportsAdded ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk1;Microsoft.NET.Sdk2\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk1;Microsoft.NET.Sdk2";

			var props = new [] { "Sdk1.props", "Sdk2.props" };
			var targets = new [] { "Sdk1.targets", "Sdk2.targets" };
			project.AddInternalSdkImports (msbuildProject, "SdkPath", props, targets);

			var firstPropertyGroup = msbuildProject.PropertyGroups.First ();
			Assert.AreEqual ("'$(MSBuildSdksPath)' == ''", firstPropertyGroup.Condition);

			var sdkPathProperty = firstPropertyGroup.GetProperty ("MSBuildSdksPath");
			Assert.AreEqual ("SdkPath", sdkPathProperty.Value);

			var createdImports = msbuildProject.Imports.Select (import => import.Project).ToArray ();
			var expectedImports = new [] { "Sdk1.props", "Sdk2.props", "Sdk1.targets", "Sdk2.targets" };
			Assert.AreEqual (expectedImports, createdImports);
		}

		[Test]
		public void WriteProject_DescriptionAdded_RemovedOnWritingSinceDefaultIsUsed ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			AddGlobalPropertyToMSBuildProject ("Description", "Package Description", string.Empty);

			WriteProject ();

			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("Description"));
		}

		[Test]
		public void WriteProject_DescriptionInOriginalProjectFile_NotRemovedOnWriting ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"      <Description>Test</Description>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();

			WriteProject ();

			Assert.IsTrue (MSBuildProjectHasGlobalProperty ("Description"));
		}

		[Test]
		public void WriteProject_TargetFrameworkInformationAdded_RemovedOnWriting ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			AddGlobalPropertyToMSBuildProject ("TargetFrameworkVersion", "1.0");
			AddGlobalPropertyToMSBuildProject ("TargetFrameworkIdentifier", ".NETCoreApp");

			WriteProject ();

			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("TargetFrameworkVersion"));
			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("TargetFrameworkIdentifier"));
		}

		[Test]
		public void WriteProject_AssemblyNameAndRootNamespaceAddedButSameAsProjectName_RemovedOnWriting ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>",
				"Test.csproj"
			);
			ReadProject ();
			AddGlobalPropertyToMSBuildProject ("AssemblyName", "Test");
			AddGlobalPropertyToMSBuildProject ("RootNamespace", "Test");

			WriteProject ();

			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("AssemblyName"));
			Assert.IsFalse (MSBuildProjectHasGlobalProperty ("RootNamespace"));
		}

		[Test]
		public void WriteProject_AssemblyNameAndRootNamespaceInOriginalProjectFile_NotRemovedOnWriting ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"      <AssemblyName>Test</AssemblyName>\r\n" +
				"      <RootNamespace>Test</RootNamespace>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();

			WriteProject ();

			Assert.IsTrue (MSBuildProjectHasGlobalProperty ("AssemblyName"));
			Assert.IsTrue (MSBuildProjectHasGlobalProperty ("RootNamespace"));
		}

		[Test]
		public void WriteProject_SdkProjectHasToolsVersionSetAfterReading_ToolsVersionRemovedOnWriting ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";
			msbuildProject.ToolsVersion = "4.0";

			WriteProject ();

			Assert.IsNull (msbuildProject.ToolsVersion);
		}

		[Test]
		public void WriteProject_NewProjectReferenceAddedWithNameAndProjectMetadata_ProjectReferenceSavedWithJustIncludeNotNameAndProject ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";
			var projectReferenceItem = msbuildProject.AddNewItem ("ProjectReference", @"Lib\Lib.csproj");
			projectReferenceItem.Metadata.SetValue ("Name", "Lib");
			projectReferenceItem.Metadata.SetValue ("Project", "{F109E7DF-F561-4CD6-A46E-CFB27A8B6F2C}");

			WriteProject ();

			var projectReferenceSaved = msbuildProject.GetAllItems ()
				.FirstOrDefault (item => item.Name == "ProjectReference");

			Assert.IsFalse (projectReferenceSaved.Metadata.HasProperty ("Name"));
			Assert.IsFalse (projectReferenceSaved.Metadata.HasProperty ("Project"));
			Assert.AreEqual (@"Lib\Lib.csproj", projectReferenceSaved.Include);
		}

		[Test]
		public void WriteProject_TargetFrameworkVersionChanged_TargetFrameworkUpdated ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			msbuildProject.Evaluate ();
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";

			WriteProject (".NETCoreApp", "1.1");

			string savedFramework = msbuildProject.GetGlobalPropertyGroup ()
				.GetValue ("TargetFramework");
			Assert.AreEqual ("netcoreapp1.1", savedFramework);
		}

		[Test]
		public void WriteProject_ProjectDefinesMultipleTargetFrameworksAndTargetFrameworkVersionChanged_TargetFrameworksUpdated ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFrameworks>netcoreapp1.0;net45</TargetFrameworks>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>");
			msbuildProject.Evaluate ();
			ReadProject ();
			project.Sdk = "Microsoft.NET.Sdk";

			WriteProject (".NETCoreApp", "1.1");

			string savedFramework = msbuildProject.GetGlobalPropertyGroup ()
				 .GetValue ("TargetFrameworks");
			Assert.AreEqual ("netcoreapp1.1;net45", savedFramework);
		}

		[Test]
		public void WriteProject_AssemblyNameAndRootNamespaceAddedDifferentToProjectName_AssemblyNameAndRootNamespaceSaved ()
		{
			CreateMSBuildProject (
				"<Project Sdk=\"Microsoft.NET.Sdk\" ToolsVersion=\"15.0\">\r\n" +
				"  <PropertyGroup>\r\n" +
				"      <OutputType>Exe</OutputType>\r\n" +
				"      <TargetFramework>netcoreapp1.0</TargetFramework>\r\n" +
				"  </PropertyGroup>\r\n" +
				"</Project>",
			"MyProject.csproj");
			ReadProject ();
			AddGlobalPropertyToMSBuildProject ("AssemblyName", "NewAssemblyName");
			AddGlobalPropertyToMSBuildProject ("RootNamespace", "NewRootNamespace");

			WriteProject ();

			Assert.AreEqual ("NewAssemblyName", GetPropertyValueFromMSBuildProject ("AssemblyName"));
			Assert.AreEqual ("NewRootNamespace", GetPropertyValueFromMSBuildProject ("RootNamespace"));
		}
	}
}
