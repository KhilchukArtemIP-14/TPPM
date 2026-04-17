using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task1.TransferStrategies
{
    public class TransferDeadlock : ITransferStrategy
    {
        public void Transfer(BankAccount fromAccount, BankAccount toAccount, decimal amount)
        {
            lock (fromAccount)
            {
                lock (toAccount)
                {
                    if (fromAccount.Balance >= amount)
                    {
                        fromAccount.Balance -= amount;
                        toAccount.Balance += amount;
                    }
                }
            }
        }
    }
}
