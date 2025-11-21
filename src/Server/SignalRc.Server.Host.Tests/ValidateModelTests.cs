using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalRc.Helpers;
using SignalRc.Models;

namespace SignalRc.Server.Host.Tests
{
    [TestClass]
    public class ValidateModelTests
    {
        [TestMethod]
        public void ShouldValidate()
        {
            var result = ValidateModel.Validate(new SignalRcModel
            {
                Self = Guid.NewGuid().ToString(),
                Version = "1.0.0",
                Vehicles = new Dictionary<string, VehicleStateModel>
                {
                    { "s", new VehicleStateModel() }
                }
            });
            result.Count().Should().Be(0);
        }

        [TestMethod]
        public void ShouldNotValidate()
        {
            var result = ValidateModel.Validate(new SignalRcModel());
            result.Count().Should().Be(3);
        }
    }
}