using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab2.Models
{
    public class Transaction
    {
        public int UserId { get; set;}
        public double Amount { get; set;}
        public string Currency { get; set;}
        public DateTime Date { get; set;}
        public string ItemType { get; set;}
    }
}
