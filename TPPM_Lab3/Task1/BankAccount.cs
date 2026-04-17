using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task1
{
    public class BankAccount
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }

        public BankAccount(int id, decimal initialBalance)
        {
            Id = id;
            Balance = initialBalance;
        }
    }
}
