using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eToroHomeAssignment
{
    public enum OperationStatus
    {
        Success,
        Failed,
        None
    }

    public class PartialCloseResultData
    {
        public string PositionId { get; set; }

        public decimal NumberOfUnitsBeforeDeduction { get; set; }

        public decimal NumberOfUnitsAfterDeduction { get; set; }

        public decimal NumberOfUnitsToDeduct { get; set; }

        public decimal UnitsRealTimePrice { get; set; } // now

        public decimal RequestedUnitsSellPrice { get; set; } // operation request time 

        public decimal PriceDrift { get; set; }

        public PartialCloseResultData(string positionId, decimal numberOfUnitsToDeduct, 
            decimal numberOfUnitsBeforeDeduction, decimal numberOfUnitsAfterDeduction, decimal unitsRealTimePrice, 
            decimal requestedUnitsSellPrice, decimal priceDrift)
        {
            PositionId = positionId;
            NumberOfUnitsBeforeDeduction = numberOfUnitsBeforeDeduction;
            NumberOfUnitsAfterDeduction = numberOfUnitsAfterDeduction;
            NumberOfUnitsToDeduct = numberOfUnitsToDeduct;
            RequestedUnitsSellPrice = requestedUnitsSellPrice;
            UnitsRealTimePrice = unitsRealTimePrice;
            PriceDrift = priceDrift;
        }
    }

    public class PartialCloseResult
    {

        public OperationStatus Status { get; init; }

        public Exception? Exception { get; init; }

        public PartialCloseResultData OperationData { get; init; }

        public PartialCloseResult (PartialCloseResultData operationData, OperationStatus status, Exception? ex = null)
        {
            OperationData = operationData;
            Status = status;
            Exception = ex;
        }
    }
}
