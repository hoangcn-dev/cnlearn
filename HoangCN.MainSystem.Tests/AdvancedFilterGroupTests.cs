using Dapper;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.DL.Utils;
using HoangCN.MainSystem.DTOs;
using HoangCN.MainSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace HoangCN.MainSystem.Tests
{
    public class AdvancedFilterGroupTests
    {
        #region Giai đoạn 1: AdvancedFilterGroup => SQL

        [Fact]
        public void BuildWhereClaude_EmptyOrNullGroup_ShouldGenerateBaseConditionOnly()
        {
            // Arrange
            var request = new GetRequest();
            var parameters = new DynamicParameters();

            // Act
            var sql = BuildSQLUtil.BuildWhereClaude<User, UserAuthDto>(request, parameters);

            // Assert
            Assert.Contains("`User`.`IsDeleted` = 0", sql);
            Assert.DoesNotContain("AND", sql);
        }

        [Fact]
        public void BuildWhereClaude_SimpleGroup_ShouldGenerateCorrectSqlAndParameters()
        {
            // Arrange
            var request = new GetRequest
            {
                AdvancedFilterGroup = new AdvancedFilterGroup
                {
                    GroupType = FilterGroupType.And,
                    Filters = new List<Filter>
                    {
                        new Filter("UserName", FilterOperator.Equal, "john", FilterType.String),
                        new Filter("IsActive", FilterOperator.Equal, true, FilterType.Bool)
                    }
                }
            };
            var parameters = new DynamicParameters();

            // Act
            var sql = BuildSQLUtil.BuildWhereClaude<User, UserAuthDto>(request, parameters);

            // Assert
            Assert.Contains("`User`.`IsDeleted` = 0", sql);
            Assert.Contains("`User`.`UserName` = @af_UserName_0", sql);
            Assert.Contains("`User`.`IsActive` = @af_IsActive_1", sql);
            
            Assert.Equal("john", parameters.Get<string>("af_UserName_0"));
            Assert.True(parameters.Get<bool>("af_IsActive_1"));
        }

        [Fact]
        public void BuildWhereClaude_RecursiveNestedGroups_ShouldGenerateSqlWithParentheses()
        {
            // Arrange
            var request = new GetRequest
            {
                AdvancedFilterGroup = new AdvancedFilterGroup
                {
                    GroupType = FilterGroupType.And,
                    Groups = new List<AdvancedFilterGroup>
                    {
                        new AdvancedFilterGroup
                        {
                            GroupType = FilterGroupType.Or,
                            Filters = new List<Filter>
                            {
                                new Filter("RoleName", FilterOperator.Equal, "Admin", FilterType.String),
                                new Filter("RoleName", FilterOperator.Equal, "User", FilterType.String)
                            }
                        },
                        new AdvancedFilterGroup
                        {
                            GroupType = FilterGroupType.And,
                            Filters = new List<Filter>
                            {
                                new Filter("IsActive", FilterOperator.Equal, true, FilterType.Bool)
                            }
                        }
                    }
                }
            };
            var parameters = new DynamicParameters();

            // Act
            var sql = BuildSQLUtil.BuildWhereClaude<User, UserAuthDto>(request, parameters);

            // Assert
            Assert.Contains("`User`.`IsDeleted` = 0", sql);
            // Cột RoleName phải phân giải chính xác sang bảng ngoại `Role` do thuộc tính trong UserAuthDto
            Assert.Contains("`Role`.`RoleName` = @af_RoleName_0", sql);
            Assert.Contains("`Role`.`RoleName` = @af_RoleName_1", sql);
            Assert.Contains("`User`.`IsActive` = @af_IsActive_2", sql);

            // Cấu trúc lồng phải được bảo tồn bằng dấu ngoặc đơn
            Assert.Contains("((`Role`.`RoleName` = @af_RoleName_0 OR `Role`.`RoleName` = @af_RoleName_1) AND (`User`.`IsActive` = @af_IsActive_2))", sql);
        }

        [Fact]
        public void BuildWhereClaude_DateAndInOperators_ShouldGenerateSqlAndParameters()
        {
            // Arrange
            var targetDate = new DateTime(2026, 6, 18);
            var names = new List<string> { "alice", "bob" };
            var request = new GetRequest
            {
                AdvancedFilterGroup = new AdvancedFilterGroup
                {
                    GroupType = FilterGroupType.And,
                    Filters = new List<Filter>
                    {
                        new Filter("CreatedDate", FilterOperator.GreaterThan, targetDate, FilterType.Date),
                        new Filter("UserName", FilterOperator.In, names, FilterType.String)
                    }
                }
            };
            var parameters = new DynamicParameters();

            // Act
            var sql = BuildSQLUtil.BuildWhereClaude<User, UserAuthDto>(request, parameters);

            // Assert
            Assert.Contains("`User`.`CreatedDate` > @af_CreatedDate_0", sql);
            Assert.Contains("`User`.`UserName` IN @af_UserName_1", sql);

            Assert.Equal(targetDate, parameters.Get<DateTime>("af_CreatedDate_0"));
            Assert.Equal(names, parameters.Get<List<string>>("af_UserName_1"));
        }

        [Fact]
        public void BuildWhereClaude_ColumnToCompare_ShouldGenerateDirectColumnComparisonWithoutParameters()
        {
            // Arrange
            var request = new GetRequest
            {
                AdvancedFilterGroup = new AdvancedFilterGroup
                {
                    GroupType = FilterGroupType.And,
                    Filters = new List<Filter>
                    {
                        new Filter("ModifiedDate", FilterOperator.GreaterThan, null, FilterType.Date, "CreatedDate")
                    }
                }
            };
            var parameters = new DynamicParameters();

            // Act
            var sql = BuildSQLUtil.BuildWhereClaude<User, UserAuthDto>(request, parameters);

            // Assert
            // Phải so sánh ModifiedDate của bảng User với CreatedDate của bảng User trực tiếp
            Assert.Contains("`User`.`ModifiedDate` > `User`.`CreatedDate`", sql);
            Assert.Empty(parameters.ParameterNames);
        }

        [Fact]
        public void BuildWhereClaude_NullValueFilter_ShouldAllowNullValue()
        {
            // Arrange
            var request = new GetRequest
            {
                AdvancedFilterGroup = new AdvancedFilterGroup
                {
                    GroupType = FilterGroupType.And,
                    Filters = new List<Filter>
                    {
                        new Filter("DisplayName", FilterOperator.Equal, null, FilterType.String),
                        new Filter("Email", FilterOperator.NotEqual, null, FilterType.String)
                    }
                }
            };
            var parameters = new DynamicParameters();

            // Act
            var sql = BuildSQLUtil.BuildWhereClaude<User, UserAuthDto>(request, parameters);

            // Assert
            Assert.Contains("`User`.`DisplayName` IS NULL", sql);
            Assert.Contains("`User`.`Email` IS NOT NULL", sql);
            Assert.Empty(parameters.ParameterNames);
        }

        #endregion

        #region Giai đoạn 2: C# LINQ Condition => AdvancedFilterGroup

        [Fact]
        public void Translate_LogicalAndOrExpression_ShouldReturnCorrectTreeStructure()
        {
            // Arrange
            Expression<Func<User, bool>> expression = u => u.IsActive && (u.UserCode == "A" || u.UserCode == "B");

            // Act
            var group = ExpressionToFilterTranslator.Translate(expression);

            // Assert
            Assert.NotNull(group);
            Assert.Equal(FilterGroupType.And, group.GroupType);
            Assert.NotNull(group.Groups);
            Assert.Equal(2, group.Groups.Count);

            var left = group.Groups[0];
            Assert.NotNull(left.Filters);
            Assert.Single(left.Filters);
            Assert.Equal("IsActive", left.Filters[0].Property);
            Assert.Equal(FilterOperator.Equal, left.Filters[0].Operator);
            Assert.True((bool)left.Filters[0].Value!);

            var right = group.Groups[1];
            Assert.Equal(FilterGroupType.Or, right.GroupType);
            Assert.NotNull(right.Groups);
            Assert.Equal(2, right.Groups.Count);
            Assert.Equal("UserCode", right.Groups[0].Filters![0].Property);
            Assert.Equal("A", right.Groups[0].Filters![0].Value);
            Assert.Equal("UserCode", right.Groups[1].Filters![0].Property);
            Assert.Equal("B", right.Groups[1].Filters![0].Value);
        }

        [Fact]
        public void Translate_BooleanMemberDirectAndNot_ShouldReturnCorrectFilters()
        {
            // Arrange
            Expression<Func<User, bool>> expressionDirect = u => u.IsActive;
            Expression<Func<User, bool>> expressionNegated = u => !u.IsActive;

            // Act
            var groupDirect = ExpressionToFilterTranslator.Translate(expressionDirect);
            var groupNegated = ExpressionToFilterTranslator.Translate(expressionNegated);

            // Assert
            Assert.Single(groupDirect.Filters!);
            Assert.Equal("IsActive", groupDirect.Filters![0].Property);
            Assert.Equal(FilterOperator.Equal, groupDirect.Filters![0].Operator);
            Assert.True((bool)groupDirect.Filters![0].Value!);

            Assert.Single(groupNegated.Filters!);
            Assert.Equal("IsActive", groupNegated.Filters![0].Property);
            Assert.Equal(FilterOperator.Equal, groupNegated.Filters![0].Operator);
            Assert.False((bool)groupNegated.Filters![0].Value!);
        }

        [Fact]
        public void Translate_StringMethods_ShouldMapToCorrectOperators()
        {
            // Arrange
            Expression<Func<User, bool>> containsExpr = u => u.UserName.Contains("admin");
            Expression<Func<User, bool>> startsExpr = u => u.Email.StartsWith("test");
            Expression<Func<User, bool>> endsExpr = u => u.Email.EndsWith(".com");

            // Act
            var groupContains = ExpressionToFilterTranslator.Translate(containsExpr);
            var groupStarts = ExpressionToFilterTranslator.Translate(startsExpr);
            var groupEnds = ExpressionToFilterTranslator.Translate(endsExpr);

            // Assert
            Assert.Equal(FilterOperator.Contain, groupContains.Filters![0].Operator);
            Assert.Equal("admin", groupContains.Filters![0].Value);

            Assert.Equal(FilterOperator.StartWith, groupStarts.Filters![0].Operator);
            Assert.Equal("test", groupStarts.Filters![0].Value);

            Assert.Equal(FilterOperator.EndWith, groupEnds.Filters![0].Operator);
            Assert.Equal(".com", groupEnds.Filters![0].Value);
        }

        [Fact]
        public void Translate_ListContains_ShouldMapToInOperator()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            Expression<Func<User, bool>> expression = u => ids.Contains(u.RoleId);

            // Act
            var group = ExpressionToFilterTranslator.Translate(expression);

            // Assert
            Assert.Single(group.Filters!);
            var filter = group.Filters![0];
            Assert.Equal("RoleId", filter.Property);
            Assert.Equal(FilterOperator.In, filter.Operator);
            Assert.Equal(ids, filter.Value);
        }

        [Fact]
        public void Translate_LocalVariablesCapturedInClosure_ShouldEvaluateCorrectlyWithoutJIT()
        {
            // Arrange
            var searchEmail = "test@domain.com";
            var minDate = new DateTime(2026, 1, 1);
            Expression<Func<User, bool>> expression = u => u.Email == searchEmail && u.CreatedDate > minDate;

            // Act
            var group = ExpressionToFilterTranslator.Translate(expression);

            // Assert
            Assert.Equal(FilterGroupType.And, group.GroupType);
            Assert.Equal("Email", group.Groups![0].Filters![0].Property);
            Assert.Equal("test@domain.com", group.Groups![0].Filters![0].Value);
            
            Assert.Equal("CreatedDate", group.Groups![1].Filters![0].Property);
            Assert.Equal(minDate, group.Groups![1].Filters![0].Value);
        }

        [Fact]
        public void Translate_ColumnToColumnComparisonExpression_ShouldTranslateToColumnToCompare()
        {
            // Arrange
            Expression<Func<User, bool>> expression = u => u.ModifiedDate > u.CreatedDate;

            // Act
            var group = ExpressionToFilterTranslator.Translate(expression);

            // Assert
            Assert.Single(group.Filters!);
            var filter = group.Filters![0];
            Assert.Equal("ModifiedDate", filter.Property);
            Assert.Equal("CreatedDate", filter.ColumnToCompare);
            Assert.Null(filter.Value);
            Assert.Equal(FilterOperator.GreaterThan, filter.Operator);
        }

        #endregion

        #region Giai đoạn 3: BuildQueryStringGetDtoByCondition Tests

        [Fact]
        public void BuildQueryStringGetDtoByCondition_WithOnlyOneTrue_ShouldGenerateSqlWithLimit1()
        {
            // Arrange
            var parameters = new DynamicParameters();
            Expression<Func<User, bool>> condition = u => u.UserName == "john";

            // Act
            var sql = BuildSQLUtil.BuildQueryStringGetDtoByCondition<User, UserAuthDto>(
                isGetOnlyOne: true,
                condition,
                parameters);

            // Assert
            Assert.Contains("SELECT", sql);
            Assert.Contains("WHERE", sql);
            Assert.Contains("`User`.`UserName` = @af_UserName_0", sql);
            Assert.Contains("LIMIT 1", sql);
            Assert.DoesNotContain("WHERE WHERE", sql);
            Assert.Equal("john", parameters.Get<string>("af_UserName_0"));
        }

        [Fact]
        public void BuildQueryStringGetDtoByCondition_WithOnlyOneFalse_ShouldGenerateSqlWithoutLimit1()
        {
            // Arrange
            var parameters = new DynamicParameters();
            Expression<Func<User, bool>> condition = u => u.UserName == "john";

            // Act
            var sql = BuildSQLUtil.BuildQueryStringGetDtoByCondition<User, UserAuthDto>(
                isGetOnlyOne: false,
                condition,
                parameters);

            // Assert
            Assert.Contains("SELECT", sql);
            Assert.Contains("WHERE", sql);
            Assert.Contains("`User`.`UserName` = @af_UserName_0", sql);
            Assert.DoesNotContain("LIMIT 1", sql);
            Assert.DoesNotContain("WHERE WHERE", sql);
            Assert.Equal("john", parameters.Get<string>("af_UserName_0"));
        }

        [Fact]
        public void BuildQueryStringGetDtoByCondition_WithForeignTableDto_ShouldGenerateSelectWithJoinsAndColumns()
        {
            // Arrange
            var parameters = new DynamicParameters();
            var targetUserId = Guid.NewGuid();
            Expression<Func<User, bool>> condition = u => u.UserId == targetUserId;

            // Act
            var sql = BuildSQLUtil.BuildQueryStringGetDtoByCondition<User, UserAuthDto>(
                isGetOnlyOne: true,
                condition,
                parameters);

            // Assert
            // 1. Phải có SELECT và lấy các cột từ bảng chính User
            Assert.Contains("SELECT", sql);
            Assert.Contains("`User`.`UserName` AS `UserName`", sql);

            // 2. Phải phân giải và lấy cột từ bảng ngoại Role
            Assert.Contains("`Role`.`RoleName` AS `RoleName`", sql);

            // 3. Phải sinh câu lệnh LEFT JOIN đến bảng Role thông qua mối quan hệ khóa ngoại RoleId
            Assert.Contains("LEFT JOIN `Role` ON `User`.`RoleId` = `Role`.`RoleId`", sql);

            // 4. Mệnh đề WHERE phải lọc đúng theo ID truyền vào
            Assert.Contains("WHERE", sql);
            Assert.Contains("`User`.`UserId` = @af_UserId_0", sql);
            Assert.Contains("LIMIT 1", sql);

            Assert.Equal(targetUserId.ToString(), parameters.Get<string>("af_UserId_0"));
        }

        #endregion
    }
}
