// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;
using Xunit;
using Xunit.Extensions;

namespace System.Web.Http.OData.Query
{
    public class ODataPathParserTest
    {
        private static IEdmModel _model;
        private static ODataPathParser _parser = new ODataPathParser();

        [Fact]
        public void CanParseUrlWithNoModelElements()
        {
            // Arrange
            string testUrl = "http://myservice/1/2()/3/4()/5";

            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.Null(segment);
        }

        [Fact]
        public void CanParseMetadataUrl()
        {
            // Arrange
            string testUrl = "http://myservice/$metadata";
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.Equal("$metadata", segment.Text);
            Assert.Null(segment.EntitySet);
            Assert.Null(segment.EdmType);
            Assert.NotNull(segment.Previous);
            Assert.Equal("http://myservice/", segment.Previous.Text);
        }

        [Fact]
        public void CanParseBatchUrl()
        {
            // Arrange
            string testUrl = "http://myservice/$batch";
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.Equal("$batch", segment.Text);
            Assert.Null(segment.EntitySet);
            Assert.Null(segment.EdmType);
            Assert.NotNull(segment.Previous);
            Assert.Equal("http://myservice/", segment.Previous.Text);
        }

        [Fact]
        public void CanParseEntitySetUrl()
        {
            // Arrange
            string testUrl = "http://myservice/Customers";
            string expectedText = "Customers";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Customers");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EdmElement);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Same(expectedSet.ElementType, (segment.EdmType as IEdmCollectionType).ElementType.Definition);
        }

        [Fact]
        public void CanParseKeyUrl()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(112)";
            string expectedText = "(112)";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Customers");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            KeyValue value = segment.EdmElement as KeyValue;
            Assert.NotNull(value);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Same(expectedSet.ElementType, segment.EdmType);
        }

        [Fact]
        public void CanParseCastCollectionSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers/System.Web.Http.OData.Query.VIP";
            string expectedText = "System.Web.Http.OData.Query.VIP";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Customers");
            IEdmEntityType expectedType = GetModel().SchemaElements.OfType<IEdmEntityType>().SingleOrDefault(s => s.Name == "VIP");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Equal(expectedType, (segment.EdmType as IEdmCollectionType).ElementType.Definition);
            Assert.Same(expectedType, segment.EdmElement);
        }

        [Fact]
        public void CanParseCastEntitySegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(100)/System.Web.Http.OData.Query.VIP";
            string expectedText = "System.Web.Http.OData.Query.VIP";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Customers");
            IEdmEntityType expectedType = GetModel().SchemaElements.OfType<IEdmEntityType>().SingleOrDefault(s => s.Name == "VIP");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Equal(expectedType, segment.EdmType);
            Assert.Same(expectedType, segment.EdmElement);
        }

        [Fact]
        public void CanParseNavigateToCollectionSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(100)/Products";
            string expectedText = "Products";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Products");
            IEdmNavigationProperty expectedEdmElement = GetModel().SchemaElements.OfType<IEdmEntityType>().SingleOrDefault(s => s.Name == "Customer").NavigationProperties().SingleOrDefault(n => n.Name == "Products");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Equal(expectedSet.ElementType, (segment.EdmType as IEdmCollectionType).ElementType.Definition);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParseNavigateToSingleSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(100)/System.Web.Http.OData.Query.VIP/RelationshipManager";
            string expectedText = "RelationshipManager";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "SalesPeople");
            IEdmNavigationProperty expectedEdmElement = GetModel().SchemaElements.OfType<IEdmEntityType>().SingleOrDefault(s => s.Name == "VIP").NavigationProperties().SingleOrDefault(n => n.Name == "RelationshipManager");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Equal(expectedSet.ElementType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParseRootProcedureSegment()
        {
            // Arrange
            string testUrl = "http://myservice/GetCustomerById()";
            string expectedText = "GetCustomerById";
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Customers");
            IEdmFunctionImport expectedEdmElement = GetModel().EntityContainers().First().FunctionImports().SingleOrDefault(s => s.Name == "GetCustomerById");
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Equal(expectedSet.ElementType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParsePropertySegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(112)/Name";
            string expectedText = "Name";
            IEdmProperty expectedEdmElement = GetModel().SchemaElements.OfType<IEdmEntityType>().SingleOrDefault(e => e.Name == "Customer").Properties().SingleOrDefault(p => p.Name == "Name");
            IEdmType expectedType = expectedEdmElement.Type.Definition;
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Null(segment.EntitySet);
            Assert.Same(expectedType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParseComplexPropertySegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(112)/Address";
            string expectedText = "Address";
            IEdmProperty expectedEdmElement = GetModel().SchemaElements.OfType<IEdmEntityType>().SingleOrDefault(e => e.Name == "Customer").Properties().SingleOrDefault(p => p.Name == "Address");
            IEdmType expectedType = expectedEdmElement.Type.Definition;
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Null(segment.EntitySet);
            Assert.Same(expectedType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParsePropertyOfComplexSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(112)/Address/Street";
            string expectedText = "Street";
            IEdmProperty expectedEdmElement = GetModel().SchemaElements.OfType<IEdmComplexType>().SingleOrDefault(e => e.Name == "Address").Properties().SingleOrDefault(p => p.Name == "Street");
            IEdmType expectedType = expectedEdmElement.Type.Definition;
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Null(segment.EntitySet);
            Assert.Same(expectedType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParsePropertyValueSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(1)/Name/$value";
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.Equal("$value", segment.Text);
            Assert.Null(segment.EntitySet);
            Assert.NotNull(segment.EdmType);
            Assert.Equal("Edm.String", (segment.EdmType as IEdmPrimitiveType).FullName());
        }

        [Fact]
        public void CanParseEntityLinksSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(1)/$links/Products";
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(s => s.Name == "Products");
            IEdmEntityType expectedType = expectedSet.ElementType;

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.Same(expectedType, (segment.EdmType as IEdmCollectionType).ElementType.Definition);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Same("$links", segment.Previous.Text);
        }

        [Fact]
        public void CanParseActionBoundToEntitySegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers(112)/GetRelatedCustomers";
            string expectedText = "GetRelatedCustomers";
            IEdmFunctionImport expectedEdmElement = GetModel().EntityContainers().First().FunctionImports().SingleOrDefault(p => p.Name == "GetRelatedCustomers");
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(e => e.Name == "Customers");
            IEdmType expectedType = expectedEdmElement.ReturnType.Definition;
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Same(expectedType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Fact]
        public void CanParseActionBoundToCollectionSegment()
        {
            // Arrange
            string testUrl = "http://myservice/Customers/System.Web.Http.OData.Query.VIP/GetMostProfitable";
            string expectedText = "GetMostProfitable";
            IEdmFunctionImport expectedEdmElement = GetModel().EntityContainers().First().FunctionImports().SingleOrDefault(p => p.Name == "GetMostProfitable");
            IEdmEntitySet expectedSet = GetModel().EntityContainers().First().EntitySets().SingleOrDefault(e => e.Name == "Customers");
            IEdmType expectedType = expectedEdmElement.ReturnType.Definition;
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            // Act
            ODataPathSegment segment = GetParser().Parse(uri, baseUri, GetModel());

            // Assert
            Assert.NotNull(segment);
            Assert.Same(GetModel(), segment.Model);
            Assert.Same(GetModel().EntityContainers().First(), segment.Container);
            Assert.NotNull(segment.Previous);
            Assert.Equal(expectedText, segment.Text);
            Assert.Same(expectedSet, segment.EntitySet);
            Assert.Same(expectedType, segment.EdmType);
            Assert.Same(expectedEdmElement, segment.EdmElement);
        }

        [Theory]
        [InlineData("http://myservice/Customers", "Customers", "Customer", true)]
        [InlineData("http://myservice/Customers/", "Customers", "Customer", true)]
        [InlineData("http://myservice/Products", "Products", "Product", true)]
        [InlineData("http://myservice/Products/", "Products", "Product", true)]
        [InlineData("http://myservice/SalesPeople", "SalesPeople", "SalesPerson", true)]
        public void CanResolveSetAndTypeViaSimpleEntitySetSegment(string testUrl, string expectedSetName, string expectedTypeName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType("System.Web.Http.OData.Query." + expectedTypeName) as IEdmEntityType;
            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);
            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        [Theory]
        [InlineData("http://myservice/Customers(1)", "Customers", "Customer", false)]
        [InlineData("http://myservice/Customers(1)/", "Customers", "Customer", false)]
        [InlineData("http://myservice/Products(1)", "Products", "Product", false)]
        [InlineData("http://myservice/Products(1)/", "Products", "Product", false)]
        [InlineData("http://myservice/Products(1)", "Products", "Product", false)]
        public void CanResolveSetAndTypeViaKeySegment(string testUrl, string expectedSetName, string expectedTypeName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType("System.Web.Http.OData.Query." + expectedTypeName) as IEdmEntityType;
            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);
            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        [Theory]
        [InlineData("http://myservice/Customers(1)/Products", "Products", "Product", true)]
        [InlineData("http://myservice/Customers(1)/Products(1)", "Products", "Product", false)]
        [InlineData("http://myservice/Customers(1)/Products/", "Products", "Product", true)]
        [InlineData("http://myservice/Customers(1)/Products", "Products", "Product", true)]
        [InlineData("http://myservice/Products(1)/Customers", "Customers", "Customer", true)]
        [InlineData("http://myservice/Products(1)/Customers(1)", "Customers", "Customer", false)]
        [InlineData("http://myservice/Products(1)/Customers/", "Customers", "Customer", true)]
        public void CanResolveSetAndTypeViaNavigationPropertySegment(string testUrl, string expectedSetName, string expectedTypeName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType("System.Web.Http.OData.Query." + expectedTypeName) as IEdmEntityType;

            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);

            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        [Theory]
        [InlineData("http://myservice/Customers/System.Web.Http.OData.Query.VIP", "VIP", "Customers", true)]
        [InlineData("http://myservice/Customers(1)/System.Web.Http.OData.Query.VIP", "VIP", "Customers", false)]
        [InlineData("http://myservice/Products(1)/System.Web.Http.OData.Query.ImportantProduct", "ImportantProduct", "Products", false)]
        [InlineData("http://myservice/Products(1)/Customers/System.Web.Http.OData.Query.VIP", "VIP", "Customers", true)]
        [InlineData("http://myservice/SalesPeople(1)/ManagedCustomers", "VIP", "Customers", true)]
        [InlineData("http://myservice/Customers(1)/System.Web.Http.OData.Query.VIP/RelationshipManager", "SalesPerson", "SalesPeople", false)]
        [InlineData("http://myservice/Products/System.Web.Http.OData.Query.ImportantProduct(1)/LeadSalesPerson", "SalesPerson", "SalesPeople", false)]
        [InlineData("http://myservice/Products(1)/Customers/System.Web.Http.OData.Query.VIP(1)/RelationshipManager/ManagedProducts", "ImportantProduct", "Products", true)]
        public void CanResolveSetAndTypeViaCastSegment(string testUrl, string expectedTypeName, string expectedSetName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType("System.Web.Http.OData.Query." + expectedTypeName) as IEdmEntityType;
            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);
            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        [Theory]
        [InlineData("http://myservice/GetCustomerById", "System.Web.Http.OData.Query.Customer", "Customers", false)]
        [InlineData("http://myservice/GetSalesPersonById", "System.Web.Http.OData.Query.SalesPerson", "SalesPeople", false)]
        [InlineData("http://myservice/GetAllVIPs", "System.Web.Http.OData.Query.VIP", "Customers", true)]
        public void CanResolveSetAndTypeViaRootProcedureSegment(string testUrl, string expectedTypeName, string expectedSetName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");

            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType(expectedTypeName) as IEdmEntityType;
            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);
            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        [Theory]
        [InlineData("http://myservice/Customers(1)/GetRelatedCustomers", "System.Web.Http.OData.Query.Customer", "Customers", true)]
        [InlineData("http://myservice/Customers(1)/GetBestRelatedCustomer", "System.Web.Http.OData.Query.VIP", "Customers", false)]
        [InlineData("http://myservice/Customers(1)/System.Web.Http.OData.Query.VIP/GetSalesPerson", "System.Web.Http.OData.Query.SalesPerson", "SalesPeople", false)]
        [InlineData("http://myservice/SalesPeople(1)/GetVIPCustomers", "System.Web.Http.OData.Query.VIP", "Customers", true)]
        public void CanResolveSetAndTypeViaEntityActionSegment(string testUrl, string expectedTypeName, string expectedSetName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType(expectedTypeName) as IEdmEntityType;

            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);

            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        [Theory]
        [InlineData("http://myservice/Customers/GetVIPs", "System.Web.Http.OData.Query.VIP", "Customers", true)]
        [InlineData("http://myservice/Customers/GetProducts", "System.Web.Http.OData.Query.Product", "Products", true)]
        [InlineData("http://myservice/Products(1)/Customers/System.Web.Http.OData.Query.VIP/GetSalesPeople", "System.Web.Http.OData.Query.SalesPerson", "SalesPeople", true)]
        [InlineData("http://myservice/SalesPeople/GetVIPCustomers", "System.Web.Http.OData.Query.VIP", "Customers", true)]
        [InlineData("http://myservice/Customers/System.Web.Http.OData.Query.VIP/GetMostProfitable", "System.Web.Http.OData.Query.VIP", "Customers", false)]
        public void CanResolveSetAndTypeViaCollectionActionSegment(string testUrl, string expectedTypeName, string expectedSetName, bool isCollection)
        {
            // Arrange
            Uri uri = new Uri(testUrl);
            Uri baseUri = new Uri("http://myservice/");
            var model = GetModel();
            var expectedSet = model.FindDeclaredEntityContainer("Container").FindEntitySet(expectedSetName);
            var expectedType = model.FindDeclaredType(expectedTypeName) as IEdmEntityType;

            // Act
            ODataPathSegment segmentInfo = GetParser().Parse(uri, baseUri, model);

            // Assert
            Assert.NotNull(segmentInfo);
            Assert.NotNull(segmentInfo.EntitySet);
            Assert.NotNull(segmentInfo.EdmType);
            Assert.Same(expectedSet, segmentInfo.EntitySet);
            if (isCollection)
            {
                Assert.Equal(EdmTypeKind.Collection, segmentInfo.EdmType.TypeKind);
                Assert.Same(expectedType, (segmentInfo.EdmType as IEdmCollectionType).ElementType.Definition);
            }
            else
            {
                Assert.Same(expectedType, segmentInfo.EdmType);
            }
        }

        public static ODataPathParser GetParser()
        {
            return _parser;
        }
        public static IEdmModel GetModel()
        {
            if (_model == null)
            {
                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
                builder.EntitySet<Customer>("Customers");
                builder.EntitySet<Product>("Products");
                builder.EntitySet<SalesPerson>("SalesPeople");
                builder.EntitySet<EmailAddress>("EmailAddresses");

                ActionConfiguration getCustomerById = new ActionConfiguration(builder, "GetCustomerById");
                getCustomerById.Parameter<int>("customerId");
                getCustomerById.ReturnsFromEntitySet<Customer>("Customers");

                ActionConfiguration getSalesPersonById = new ActionConfiguration(builder, "GetSalesPersonById");
                getSalesPersonById.Parameter<int>("salesPersonId");
                getSalesPersonById.ReturnsFromEntitySet<SalesPerson>("SalesPeople");

                ActionConfiguration getAllVIPs = new ActionConfiguration(builder, "GetAllVIPs");
                builder.ActionReturnsCollectionFromEntitySet<VIP>(getAllVIPs, "Customers");

                builder.Entity<Customer>().ComplexProperty<Address>(c => c.Address);
                builder.Entity<Customer>().Action("GetRelatedCustomers").ReturnsCollectionFromEntitySet<Customer>("Customers");

                ActionConfiguration getBestRelatedCustomer = builder.Entity<Customer>().Action("GetBestRelatedCustomer");
                builder.ActionReturnsFromEntitySet<VIP>(getBestRelatedCustomer, "Customers");

                ActionConfiguration getVIPS = builder.Entity<Customer>().Collection.Action("GetVIPs");
                builder.ActionReturnsCollectionFromEntitySet<VIP>(getVIPS, "Customers");

                builder.Entity<Customer>().Collection.Action("GetProducts").ReturnsCollectionFromEntitySet<Product>("Products");
                builder.Entity<VIP>().Action("GetSalesPerson").ReturnsFromEntitySet<SalesPerson>("SalesPeople");
                builder.Entity<VIP>().Collection.Action("GetSalesPeople").ReturnsCollectionFromEntitySet<SalesPerson>("SalesPeople");

                ActionConfiguration getMostProfitable = builder.Entity<VIP>().Collection.Action("GetMostProfitable");
                builder.ActionReturnsFromEntitySet<VIP>(getMostProfitable, "Customers");

                ActionConfiguration getVIPCustomers = builder.Entity<SalesPerson>().Action("GetVIPCustomers");
                builder.ActionReturnsCollectionFromEntitySet<VIP>(getVIPCustomers, "Customers");

                ActionConfiguration getVIPCustomersOnCollection = builder.Entity<SalesPerson>().Collection.Action("GetVIPCustomers");
                builder.ActionReturnsCollectionFromEntitySet<VIP>(getVIPCustomersOnCollection, "Customers");

                //TODO: These calls should happen automatically - but for some reason are required.
                builder.Entity<VIP>().HasRequired(v => v.RelationshipManager);
                builder.Entity<ImportantProduct>().HasRequired(ip => ip.LeadSalesPerson);

                _model = builder.GetEdmModel();
            }
            return _model;
        }

        public class Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public virtual List<Product> Products { get; set; }
            public Address Address { get; set; }
        }

        public class EmailAddress
        {
            [Key]
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
        }

        public class Product
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public virtual List<Customer> Customers { get; set; }
        }

        public class SalesPerson
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public virtual List<VIP> ManagedCustomers { get; set; }
            public virtual List<ImportantProduct> ManagedProducts { get; set; }
        }

        public class VIP : Customer
        {
            public virtual SalesPerson RelationshipManager { get; set; }
        }

        public class ImportantProduct : Product
        {
            public virtual SalesPerson LeadSalesPerson { get; set; }
        }
    }
}
