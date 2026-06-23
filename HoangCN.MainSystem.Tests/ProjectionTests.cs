using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Utils;
using HoangCN.Core.Common.Exceptions;
using Xunit;

namespace HoangCN.MainSystem.Tests
{
    public class FakeEntityForTest : BaseEntity
    {
        public Guid FakeEntityForTestId { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public List<string> ComplexItems { get; set; } = new();
    }

    public class FakeDtoForTest
    {
        public Guid FakeEntityForTestId { get; set; }
        public string Name { get; set; } = null!;
        public string ComplexItemsJsonData { get; set; } = null!;
    }

    public class ProjectionTests
    {
        #region ExpressionToSelectColumnsTranslator Tests

        [Fact]
        public void Translate_Should_ReturnPropertyNames_When_AnonymousTypeIsUsed()
        {
            // Arrange
            Expression<Func<FakeEntityForTest, object>> selector = x => new { x.FakeEntityForTestId, x.Name };

            // Act
            var result = ExpressionToSelectColumnsTranslator.Translate(selector);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("FakeEntityForTestId", result);
            Assert.Contains("Name", result);
        }

        [Fact]
        public void Translate_Should_ReturnSinglePropertyName_When_SinglePropertyIsUsed()
        {
            // Arrange
            Expression<Func<FakeEntityForTest, object>> selector = x => x.Name;

            // Act
            var result = ExpressionToSelectColumnsTranslator.Translate(selector);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Name", result[0]);
        }

        [Fact]
        public void Translate_Should_ReturnSinglePropertyNameForValueType_When_SingleValuePropertyIsUsed()
        {
            // Arrange
            Expression<Func<FakeEntityForTest, object>> selector = x => x.FakeEntityForTestId;

            // Act
            var result = ExpressionToSelectColumnsTranslator.Translate(selector);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("FakeEntityForTestId", result[0]);
        }

        [Fact]
        public void Translate_Should_ThrowArgumentException_When_ExpressionIsInvalid()
        {
            // Arrange
            Expression<Func<FakeEntityForTest, object>> selector = x => x.Name.Length;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ExpressionToSelectColumnsTranslator.Translate(selector));
        }

        #endregion

        #region BuildSelectClaude Tests

        [Fact]
        public void BuildSelectClaude_Should_GenerateSelectiveSql_When_SelectedPropertiesAreProvided()
        {
            // Arrange
            var selectedProperties = new List<string> { "FakeEntityForTestId", "Name" };

            // Act
            var sql = BuildSQLUtil.BuildSelectClaude<FakeEntityForTest, FakeDtoForTest>(selectedProperties);

            // Assert
            Assert.Contains("`FakeEntityForTest`.`FakeEntityForTestId` AS `FakeEntityForTestId`", sql);
            Assert.Contains("`FakeEntityForTest`.`Name` AS `Name`", sql);
            Assert.DoesNotContain("`FakeEntityForTest`.`Age`", sql);
        }

        [Fact]
        public void BuildSelectClaude_Should_MapComplexTypeToJsonDataColumn_When_SelectedPropertyIsComplex()
        {
            // Arrange
            var selectedProperties = new List<string> { "ComplexItems" };

            // Act
            var sql = BuildSQLUtil.BuildSelectClaude<FakeEntityForTest, FakeDtoForTest>(selectedProperties);

            // Assert
            Assert.Contains("`FakeEntityForTest`.`ComplexItemsJsonData` AS `ComplexItemsJsonData`", sql);
        }

        [Fact]
        public void BuildSelectClaude_Should_ThrowBadRequestException_When_PropertyDoesNotExistInEntity()
        {
            // Arrange
            var selectedProperties = new List<string> { "NonExistingProperty" };

            // Act & Assert
            Assert.Throws<BadRequestException>(() => 
                BuildSQLUtil.BuildSelectClaude<FakeEntityForTest, FakeDtoForTest>(selectedProperties));
        }

        #endregion
    }
}
