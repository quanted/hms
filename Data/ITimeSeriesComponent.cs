using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public interface ITimeSeriesComponent
    {

        ITimeSeries Output { get; set; }
        ITimeSeriesInput Input { get; set; }

    }
}
