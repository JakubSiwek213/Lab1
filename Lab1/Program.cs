using System;



namespace Lab1_Task.ConsoleApp
{
    public class BankAccount
    {
        private string _accountNumber;
        private string _ownerName;
        private string _currency;
        private decimal _balance;
        private decimal _dailyWithdrawLimit;
        private bool _isActive;



        public string AccountNumber
        {
            get => _accountNumber;
            set => _accountNumber = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Numer konta nie może być pusty")
                : value;
        }

        public string OwnerName

        {
            get => _ownerName;
            set => _ownerName = string.IsNullOrWhiteSpace(value)
                    ? throw new ArgumentException("Nazwa konta nie może być pusta")
                    : value;
        }

        public string Currency
        {
            get => _currency;
            set => _currency = string.IsNullOrWhiteSpace(value)
                    ? throw new ArgumentException("Uzupełnij walutę ")
                    : value;
        }

        public decimal Balance
        {
            get => _balance;
            private set => _balance = value;

        }
        public decimal DailyWithdrawLimit
        {
            get => _dailyWithdrawLimit;
            set => _dailyWithdrawLimit = value >= 0 ? value : throw new ArgumentException("Limit nie może być negatywny");

        }
        public bool isActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public BankAccount(string accountNumber, string ownerName, string currency, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            OwnerName = ownerName;
            Balance = initialBalance;
            Currency = currency;
            isActive = true;
            DailyWithdrawLimit = 500m;

        }

        public void Deposit(decimal wartosc)
        {
            if (wartosc > 0)
                Balance += wartosc;
        }

        public void Withdraw(Decimal wartosc)
        {
            if (wartosc > 0 && wartosc <= Balance)
                Balance -= wartosc;
        }

        public void TransferTo(BankAccount cel, decimal wartosc)
        {
            if (cel == null || wartosc <= 0) return;
            decimal saldopoczatkowe = Balance;
            Withdraw(wartosc);
            if (Balance < saldopoczatkowe)
                cel.Deposit(wartosc);

        }
        public void Close()
        {
            isActive = false;
            Console.WriteLine($"Account {AccountNumber} closed.");
        }
    }

}


namespace Lab1_Task.ConsoleApp
{
    internal class Program
    {
        static void Main()
        {

            BankAccount acc1 = new BankAccount("PL10", "Jan Kowalski", "PLN", 2050m);
            BankAccount acc2 = new BankAccount("PL20", "Anna Nowak", "PLN", 210m);
            BankAccount acc3 = new BankAccount("PL30", "Piotr Wiśniewski", "PLN", 10000m);


            acc1.Deposit(300m);
            Console.WriteLine($"{acc1.OwnerName} ma {acc1.Balance} {acc1.Currency} po wpłacie.");


            acc2.Withdraw(200m);
            Console.WriteLine($"{acc2.OwnerName} ma {acc2.Balance} {acc2.Currency} po wypłacie.");


            acc1.TransferTo(acc3, 500m);
            Console.WriteLine($"{acc1.OwnerName} ma {acc1.Balance} {acc1.Currency}, a {acc3.OwnerName} ma {acc3.Balance} {acc3.Currency} po przelewie.");


            acc3.Close();
            Console.WriteLine($"Czy konto {acc3.AccountNumber} jest aktywne? {acc3.isActive}");
        }
    }
}
