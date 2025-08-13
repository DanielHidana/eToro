using System.Diagnostics;

namespace eToroHomeAssignment
{
    public class PartialCloseServiceTests
    {
        private PartialCloseService _service = null!;
        private string _validPositionId;
        private int _numberOfUnitsToDeduct = 50;
        private PartialCloseServiceMockHelper helper = new PartialCloseServiceMockHelper();

        [SetUp]
        public void SetUp()
        {
            _service = new PartialCloseService();
            _validPositionId = Guid.NewGuid().ToString();
        }

        [Test]
        public void ClosePartialPosition_ValidData_ShouldReturnSuccess_NoException()
        {
            var result = _service.ClosePartialPosition(_validPositionId, _numberOfUnitsToDeduct);
            Assert.That(result.OperationData.PositionId, Is.EqualTo(_validPositionId));
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Success));
            Assert.That(result.Exception, Is.Null);
        }

        [Test]
        public void ClosePartialPosition_PositionNotFound_ShouldReturnFailed_WithInvalidOperationException()
        {
            var result = _service.ClosePartialPosition(helper.PositionNotFoundID, _numberOfUnitsToDeduct);
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Failed));
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception!.Message, Is.EqualTo("Position not found"));
        }

        [Test]
        public void ClosePartialPosition_PositionInactive_ShouldReturnFailed_WithInvalidOperationException()
        {
            var result = _service.ClosePartialPosition(helper.PositionInactiveID, _numberOfUnitsToDeduct);
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Failed));
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception!.Message, Is.EqualTo("Position not active"));
        }

        [TestCase(0)]
        [TestCase(-5)]
        [TestCase(-0.1)]
        public void ClosePartialPosition_InvalidUnits_ShouldReturnFailed_WithArgumentException(decimal unitsToDeduct)
        {
            var result = _service.ClosePartialPosition(_validPositionId, unitsToDeduct);
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Failed));
            Assert.That(result.Exception, Is.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(result.Exception!.Message, Does.Contain("unitsToDeduct must be bigger than"));
        }

        [Test]
        public void ClosePartialPosition_ExceedsMax_ShouldReturnFailed_WithArgumentOutOfRangeException()
        {
            var result = _service.ClosePartialPosition(_validPositionId, 150);
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Failed));
            Assert.That(result.Exception, Is.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(result.Exception!.Message, Does.Contain("less or equal to"));
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void ClosePartialPosition_UnitsLegalRangeWithBoundaries_ShouldReturnSuccess_NoException(decimal unitsToDeduct)
        {
            var result = _service.ClosePartialPosition(_validPositionId, unitsToDeduct);
            Assert.That(result.OperationData.PositionId, Is.EqualTo(_validPositionId));
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Success));
            Assert.That(result.Exception, Is.Null);
        }

        [Test]
        public void ClosePartialPosition_ExecutionTimeSLA_ShouldReturnSuccess_NoException()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = _service.ClosePartialPosition(_validPositionId, _numberOfUnitsToDeduct);
            var functionDuration = stopwatch.ElapsedMilliseconds;
            Assert.That(functionDuration, Is.LessThan(helper.MaxFunctionDuration_ms));
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Success));
            Assert.That(result.Exception, Is.Null);
        }

        [Test]
        public void ClosePartialPosition_ExecutionTimeExceeded_ShouldReturnFailed()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = _service.ClosePartialPosition(helper.MaxFunctionDurationId, _numberOfUnitsToDeduct);
            var functionDuration = stopwatch.ElapsedMilliseconds;
            Assert.That(functionDuration, Is.LessThan(helper.MaxFunctionDuration_ms));
        }

        [Test]
        public void ClosePartialPosition_DBNotUpdateCorrectlyId_ShouldReturnFailed()
        {
            var result = _service.ClosePartialPosition(helper.DBWrontUpdataId, _numberOfUnitsToDeduct);
            Assert.That(result.OperationData.NumberOfUnitsAfterDeduction, Is.Not.EqualTo(_numberOfUnitsToDeduct));
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Success));
            Assert.That(result.Exception, Is.Null);
        }

        [Test]
        public void ClosePartialPosition_TransactionWithAllowedPriceDrift_ShouldReturnSuccess_NoException()
        {
            var result = _service.ClosePartialPosition(_validPositionId, _numberOfUnitsToDeduct);
            Assert.That(result.OperationData.PriceDrift, Is.LessThan(helper.PriceMaxDrift));
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Success));
            Assert.That(result.Exception, Is.Null);
        }

        [Test]
        public void ClosePartialPosition_TransactionWithOutOfRangeDrift_ShouldReturnFailed_WithArgumentOutOfRangeException()
        {
            var result = _service.ClosePartialPosition(helper.OutOfRangeDriftId, _numberOfUnitsToDeduct);
            Assert.That(result.Status, Is.EqualTo(OperationStatus.Failed));
            Assert.That(result.Exception!.Message, Does.Contain("Price Drift if out of range"));
        }
    }
}