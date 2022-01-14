using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Context
{
    public class MeasurementsContextBuilder
    {
        public static MeasurementsContext BuildMeasurementsContext() => new MeasurementsContext();
    }
}
