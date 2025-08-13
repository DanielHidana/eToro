using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eToroHomeAssignment
{
    internal class PartialCloseServiceMockHelper
    {
        public string PositionInactiveID
        {
            get { return "PositionInactivId"; }
        }

        public string PositionNotFoundID
        {
            get { return "PositionNotFoundId"; }
        }

        public string FunctionDurationExceedId
        {
            get { return "FunctionDurationExceedId"; }
        }

        public string MaxFunctionDurationId
        {
            get { return "MaxFunctionDurationId"; }
        }

        public string DBWrontUpdataId
        {
            get { return "DBWrontUpdataId"; }
        }

        public string OutOfRangeDriftId
        {
            get { return "OutOfRangeDriftId"; }
        }

        public int NumberOfUnitsInPosition
        {
            get { return 100; }
        }

        public int MinNumberOfUnits
        {
            get { return 0; }
        }

        public int MaxFunctionDuration_ms
        {
            get { return 5_000; }
        }

        public decimal UnitsRealTimePrice
        {
            get { return 10m; }
        }

        public decimal RequestedUnitPrice
        {
            get { return 9.5m; }
        }

        public decimal PriceMaxDrift // 10% 
        {
            get { return 10m; }
        }
    }
}
