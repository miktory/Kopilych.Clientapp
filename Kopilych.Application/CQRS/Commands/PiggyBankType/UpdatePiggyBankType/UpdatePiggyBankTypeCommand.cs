using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBankType.UpdatePiggyBankType
{
    public class UpdatePiggyBankTypeCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstStatePhotoPath { get; set; }
        public string SecondStatePhotoPath { get; set; }
        public string ThirdStatePhotoPath { get; set; }
        public string FourthStatePhotoPath { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
