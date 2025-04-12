using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdateTransactionDTO
    {
        private string? _description;
        private DateTime? _date;
        private decimal? _amount;
        private int? _version;
        private int? _paymentTypeId;
        private int? _transactionTypeId;

        [JsonIgnore]
        public List<string> EditedFields { get; set; } = new List<string>();

        public int? Version {
            get { return _version; }
            set { _version = value ?? 0; EditedFields.Add(nameof(this.Version)); }
        }

        public int? PaymentTypeId
        {
            get { return _paymentTypeId; }
            set { _paymentTypeId = value ?? 0; EditedFields.Add(nameof(this.PaymentTypeId)); }
        }

        public int? TransactionTypeId
        {
            get { return _transactionTypeId; }
            set { _transactionTypeId = value ?? 0; EditedFields.Add(nameof(this.TransactionTypeId)); }
        }

        public decimal? Amount
        {
            get { return _amount; }
            set { _amount = value?? 0; EditedFields.Add(nameof(this.Amount)); }
        }

        public string? Description 
        {
            get { return _description; }
            set { _description = value; EditedFields.Add(nameof(this.Description)); }
        }
        public DateTime? Date 
        {
            get { return _date; }
            set { _date = value; EditedFields.Add(nameof(this.Date)); }
        }
    }
}
