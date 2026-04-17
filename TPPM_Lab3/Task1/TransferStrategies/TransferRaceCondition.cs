using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task1.TransferStrategies
{
    public class TransferRaceCondition : ITransferStrategy
    {
        public void Transfer(BankAccount fromAccount, BankAccount toAccount, decimal amount)
        {
            if (fromAccount.Balance >= amount)
            {
                decimal fromTemp = fromAccount.Balance;
                decimal toTemp = toAccount.Balance;

                fromAccount.Balance = fromTemp - amount;
                toAccount.Balance = toTemp + amount;
            }
        }
    }
}
