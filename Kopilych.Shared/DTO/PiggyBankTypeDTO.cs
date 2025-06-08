using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class PiggyBankTypeDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string? FirstStatePhotoPath { get; set; }
        public string? SecondStatePhotoPath { get; set; }
        public string? ThirdStatePhotoPath { get; set; }
        public string? FourthStatePhotoPath { get; set; }

        public override string ToString()
        {
            return Name; 
        }

        public override bool Equals(object obj)
        {
            return obj is PiggyBankTypeDTO dto && Name == dto.Name && Id == dto.Id && FirstStatePhotoPath == dto.FirstStatePhotoPath && SecondStatePhotoPath == dto.SecondStatePhotoPath && ThirdStatePhotoPath == dto.ThirdStatePhotoPath && FourthStatePhotoPath == dto.FourthStatePhotoPath;
        }

        public override int GetHashCode()
        {
            return (Name + Id.ToString() + FirstStatePhotoPath + SecondStatePhotoPath + ThirdStatePhotoPath + FourthStatePhotoPath).GetHashCode();
        }

    }
}
