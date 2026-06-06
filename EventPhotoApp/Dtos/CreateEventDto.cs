using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Dtos
{
    public class CreateEventDto
    {
        public string Name { get; set; } = "";
        public string DateOfEvent { get; set; } = "";
        public string TimeOfEvent { get; set; } = "";
    }
}
