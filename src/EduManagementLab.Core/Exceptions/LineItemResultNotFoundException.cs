using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{

    public class LineItemResultNotFoundException : Exception
    {
        public LineItemResultNotFoundException(Guid id) : base($"No Line Item Result found with id {id}")
        {
        }
    }
}
