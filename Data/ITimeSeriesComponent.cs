using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public interface ITimeSeriesComponent
    {

        ITimeSeriesOutput Output { get; set; }
        ITimeSeriesInput Input { get; set; }

    }
}
