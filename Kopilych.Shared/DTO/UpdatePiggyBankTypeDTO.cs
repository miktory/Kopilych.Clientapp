using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdatePiggyBankTypeDTO
    {
        public string? Name { get; set; }
        public string? FirstStatePhotoPath { get; set; }
        public string? SecondStatePhotoPath { get; set; }
        public string? ThirdStatePhotoPath { get; set; }
        public string? FourthStatePhotoPath { get; set; }
    }
}
