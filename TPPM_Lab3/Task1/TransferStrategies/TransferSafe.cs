using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task1.TransferStrategies
{
    public class TransferSafe : ITransferStrategy
    {
        public void Transfer(BankAccount fromAccount, BankAccount toAccount, decimal amount)
        {
            BankAccount firstLock = fromAccount.Id < toAccount.Id ? fromAccount : toAccount;
            BankAccount secondLock = fromAccount.Id < toAccount.Id ? toAccount : fromAccount;

            lock (firstLock)
            {
                lock (secondLock)
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
