using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace eToroHomeAssignment
{
    public class PartialCloseService
    {
        PartialCloseServiceMockHelper helper = new PartialCloseServiceMockHelper();

        public PartialCloseResult ClosePartialPosition(string positionId, decimal unitsToDeduct)
        {
            var drift = Math.Abs((helper.UnitsRealTimePrice - helper.RequestedUnitPrice) / helper.RequestedUnitPrice * 100m);
            var partialCloseResultData = new PartialCloseResultData(positionId, unitsToDeduct, helper.NumberOfUnitsInPosition, 
                (helper.NumberOfUnitsInPosition - unitsToDeduct), helper.UnitsRealTimePrice, helper.RequestedUnitPrice, drift);

            if (positionId == helper.PositionNotFoundID)
                return new PartialCloseResult(null!, OperationStatus.Failed, new InvalidOperationException("Position not found"));

            if (positionId == helper.PositionInactiveID)
                return new PartialCloseResult(null!, OperationStatus.Failed, new InvalidOperationException("Position not active"));

            if (unitsToDeduct <= helper.MinNumberOfUnits || unitsToDeduct > helper.NumberOfUnitsInPosition)
                return new PartialCloseResult(null!, OperationStatus.Failed, new ArgumentOutOfRangeException(
                    $"unitsToDeduct must be bigger than {helper.MinNumberOfUnits} and less or equal to {helper.NumberOfUnitsInPosition}"));

            if (positionId == helper.MaxFunctionDurationId)
                Task.Delay(helper.MaxFunctionDuration_ms);

            if (positionId == helper.DBWrontUpdataId)
            {
                partialCloseResultData.NumberOfUnitsAfterDeduction = 20;
                return new PartialCloseResult(partialCloseResultData, OperationStatus.Success);
            }

            if (positionId == helper.OutOfRangeDriftId)
            {
                return new PartialCloseResult(null!, OperationStatus.Failed, new ArgumentOutOfRangeException("Price Drift if out of range"));
            }

            return new PartialCloseResult(partialCloseResultData, OperationStatus.Success);
        }
    }
}
