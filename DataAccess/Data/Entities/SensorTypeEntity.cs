﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class SensorTypeEntity : EntityBase
    {
        public string Name { get; set; }
        public string Unit { get; set; }

    }
}
