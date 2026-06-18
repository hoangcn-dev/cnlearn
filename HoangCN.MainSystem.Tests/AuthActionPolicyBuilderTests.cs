using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HoangCN.Core.BL.Base;
using Xunit;
using HoangCN.Core.Common.Base;
using HoangCN.Core.BL.Interfaces;

namespace HoangCN.MainSystem.Tests
{
    public class FakeEntity : BaseEntity
    {
    }

    public class TestBaseController : CRUDController<FakeEntity>
    {
        public TestBaseController(IBaseBL<FakeEntity> baseBL) : base(baseBL)
        {
        }
    }

    public class TestSubController : TestBaseController
    {
        public TestSubController(IBaseBL<FakeEntity> baseBL) : base(baseBL)
        {
        }

        // Action declared only on the child controller (invalid for parent validation)
        public IActionResult SubAction()
        {
            return Ok();
        }
    }

    public class NonBaseController : ControllerBase
    {
    }

    public class AuthActionPolicyBuilderTests
    {
        [Fact]
        public void Constructor_WithNonBaseController_ThrowsArgumentException()
        {
            var policies = new Dictionary<string, AuthActionSettings>();
            Assert.Throws<ArgumentException>(() => new AuthActionPolicyBuilder(policies, typeof(NonBaseController)));
        }

        [Fact]
        public void Protect_WithParentAction_Succeeds()
        {
            var policies = new Dictionary<string, AuthActionSettings>();
            var builder = new AuthActionPolicyBuilder(policies, typeof(TestSubController));

            // 'GetAll' is defined in CRUDController (parent of TestSubController)
            builder.Protect("GetAll");

            Assert.True(policies.ContainsKey("GetAll"));
            Assert.True(policies["GetAll"].IsEnabled);
        }

        [Fact]
        public void Protect_WithChildAction_ThrowsArgumentException()
        {
            var policies = new Dictionary<string, AuthActionSettings>();
            var builder = new AuthActionPolicyBuilder(policies, typeof(TestSubController));

            // 'SubAction' is defined on the child controller itself, not on any parent controller
            Assert.Throws<ArgumentException>(() => builder.Protect("SubAction"));
        }

        [Fact]
        public void Protect_WithNonExistentAction_ThrowsArgumentException()
        {
            var policies = new Dictionary<string, AuthActionSettings>();
            var builder = new AuthActionPolicyBuilder(policies, typeof(TestSubController));

            // 'RandomActionName' does not exist anywhere in the chain
            Assert.Throws<ArgumentException>(() => builder.Protect("RandomActionName"));
        }
    }
}
