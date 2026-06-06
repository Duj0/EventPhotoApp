using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Models
{
    public class CreateEvent
    {
        required public string Name { get; set; }
        required public string TimeOfEvent { get; set; }
        required public string DateOfEvent { get; set; }
    }
}
