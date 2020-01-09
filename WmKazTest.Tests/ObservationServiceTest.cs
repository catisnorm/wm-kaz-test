using System;
using Moq;
using Moq.AutoMock;
using WmKazTest.Core.Model;
using WmKazTest.Core.Services;
using WmKazTest.Data.Interfaces;
using Xunit;

namespace WmKazTest.Tests
{
    public class ObservationServiceTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("11a1111", "0000111")]
        [InlineData("11111110", "11111")]
        [InlineData("00011101")]
        [InlineData("00011101", "00011101", "00011101")]
        public async void AddObservation_ThrowsArgumentExceptionIfNumbersAreInvalid(params string[] numbers)
        {
            var uow = new Mock<IUnitOfWork>();
            var autoMock = new AutoMocker();
            autoMock.Use(uow.Object);
            var serviceMock = autoMock.CreateInstance<ObservationService>();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                serviceMock.AddObservation(new Observation { Numbers = numbers }));
        }
    }
}