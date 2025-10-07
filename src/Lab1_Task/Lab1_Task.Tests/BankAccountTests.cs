using FluentAssertions;
using System.Reflection;

namespace Lab1_Task.Tests
{
    /// <summary>
    /// Autograding: każdy test = 1 punkt (Trait("points","1")).
    /// Założenia:
    /// - Dopuszczalne różne konstruktory (CreateInstance znajdzie coś sensownego).
    /// - Kwoty mogą być decimal/double/float/int/… (IsNumeric + konwersje).
    /// - Balance może mieć private set (SeedBalance użyje Deposit/Withdraw).
    /// - „Jedna własna właściwość” weryfikowana przez łączną liczbę właściwości (>=6).
    /// - „Min. 5 pól” zliczane także prywatne pola.
    /// </summary>
    public class BankAccountTests
    {

        private static Type BankType =>
             Type.GetType("Lab1_Task.ConsoleApp.BankAccount, Lab1_Task.ConsoleApp")
             ?? throw new InvalidOperationException("Nie znaleziono typu Lab1_Task.ConsoleApp.BankAccount – upewnij się, że namespace i assembly są poprawne.");


        //Helpers
        private static bool IsNumeric(Type t) =>
                t == typeof(decimal) || t == typeof(double) || t == typeof(float) ||
                t == typeof(long) || t == typeof(int) || t == typeof(short);

        private static object ConvertTo(Type t, decimal value)
        {
            if (t == typeof(decimal)) return value;
            if (t == typeof(double)) return (double)value;
            if (t == typeof(float)) return (float)value;
            if (t == typeof(long)) return (long)value;
            if (t == typeof(int)) return (int)value;
            if (t == typeof(short)) return (short)value;
            throw new InvalidOperationException($"Nieobsługiwany typ liczbowy: {t.Name}");
        }

        private static decimal ToDecimal(object? v) => v switch
        {
            decimal d => d,
            double d => (decimal)d,
            float f => (decimal)f,
            long l => l,
            int i => i,
            short s => s,
            _ => throw new InvalidOperationException("Balance nie jest typem liczbowym czytelnym dla testu.")
        };

        private static object CreateInstance()
        {
            //Konstruktor bezparametrowy
            var ctor = BankType.GetConstructor(Type.EmptyTypes);
            if (ctor != null) return Activator.CreateInstance(BankType)!;

            //Dostepny Konstruktor
            var anyCtor = BankType.GetConstructors()
                                  .OrderBy(c => c.GetParameters().Length)
                                  .FirstOrDefault()
                          ?? throw new InvalidOperationException("Brak dostępnego konstruktora klasy BankAccount.");

            var args = anyCtor.GetParameters().Select(p =>
            {
                if (p.ParameterType == typeof(string)) return "";
                if (p.ParameterType == typeof(bool)) return false;
                if (IsNumeric(p.ParameterType)) return ConvertTo(p.ParameterType, 0m);
                if (p.ParameterType == typeof(DateTime)) return DateTime.MinValue;
                return p.HasDefaultValue ? p.DefaultValue! :
                       (p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType)! : null);
            }).ToArray();

            return anyCtor.Invoke(args);
        }

        private static PropertyInfo GetPublicProp(string name)
        {
            var p = BankType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            p.Should().NotBeNull($"Brak właściwości publicznej: {name}");
            return p!;
        }

        private static PropertyInfo GetBalanceProp()
        {
            var prop = BankType.GetProperty("Balance", BindingFlags.Public | BindingFlags.Instance);
            prop.Should().NotBeNull("Właściwość Balance musi istnieć (publiczna).");
            prop!.GetMethod.Should().NotBeNull("Balance musi mieć publiczny getter.");
            return prop!;
        }

        private static MethodInfo GetNumericMethod(string name, int paramCount)
        {
            var m = BankType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .FirstOrDefault(mi =>
                                mi.Name == name &&
                                mi.GetParameters().Length == paramCount &&
                                (paramCount == 1
                                    ? IsNumeric(mi.GetParameters()[0].ParameterType)
                                    : (mi.GetParameters()[0].ParameterType == BankType &&
                                       IsNumeric(mi.GetParameters()[1].ParameterType))));
            m.Should().NotBeNull($"{name} o oczekiwanej sygnaturze musi istnieć.");
            return m!;
        }

        private static void SeedBalance(object account, PropertyInfo balanceProp, decimal target)
        {
            if (balanceProp.CanWrite && balanceProp.SetMethod!.IsPublic)
            {
                balanceProp.SetValue(account, ConvertTo(balanceProp.PropertyType, target));
                return;
            }

            var deposit = GetNumericMethod("Deposit", 1);
            var current = ToDecimal(balanceProp.GetValue(account));

            if (current > target)
            {
                var withdraw = GetNumericMethod("Withdraw", 1);
                var diff = current - target;
                var wType = withdraw.GetParameters()[0].ParameterType;
                withdraw.Invoke(account, new[] { ConvertTo(wType, diff) });
            }
            else if (current < target)
            {
                var diff = target - current;
                var dType = deposit.GetParameters()[0].ParameterType;
                deposit.Invoke(account, new[] { ConvertTo(dType, diff) });
            }
        }

        private static Exception? InvokeCatching(MethodInfo m, object target, params object[] args)
        {
            try { m.Invoke(target, args); return null; }
            catch (TargetInvocationException ex) { return ex.InnerException ?? ex; }
        }

        //Kontrakt

        [Trait("points", "1")]
        [Fact(DisplayName = "Klasa BankAccount istnieje i jest publiczna")]
        public void S01_ClassExists_Public()
        {
            BankType.IsClass.Should().BeTrue("BankAccount powinna być klasą.");
            BankType.IsPublic.Should().BeTrue("BankAccount powinna być publiczna.");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Właściwość AccountNumber istnieje (public)")]
        public void S02_Prop_AccountNumber_Exists()
        {
            GetPublicProp("AccountNumber");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Właściwość OwnerName istnieje (public)")]
        public void S03_Prop_OwnerName_Exists()
        {
            GetPublicProp("OwnerName");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Właściwość Balance istnieje (public getter)")]
        public void S04_Prop_Balance_Exists()
        {
            GetBalanceProp();
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Właściwość Currency istnieje (public)")]
        public void S05_Prop_Currency_Exists()
        {
            GetPublicProp("Currency");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Właściwość IsActive istnieje (public)")]
        public void S06_Prop_IsActive_Exists()
        {
            GetPublicProp("IsActive");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Jest co najmniej 6 publicznych właściwości (w tym jedna własna)")]
        public void S07_Props_Count_AtLeast6()
        {
            var props = BankType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            props.Length.Should().BeGreaterThanOrEqualTo(6, "wymagane 5 właściwości z treści + 1 własna");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Jest co najmniej 5 pól (mogą być prywatne)")]
        public void S08_Fields_Count_AtLeast5()
        {
            var fields = BankType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            fields.Length.Should().BeGreaterThanOrEqualTo(5, "wymagane co najmniej 5 pól (walidacje często w polach prywatnych)");
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Metoda Deposit(amount) istnieje (amount – typ liczbowy)")]
        public void S09_Method_Deposit_Exists()
        {
            GetNumericMethod("Deposit", 1);
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Metoda Withdraw(amount) istnieje (amount – typ liczbowy)")]
        public void S10_Method_Withdraw_Exists()
        {
            GetNumericMethod("Withdraw", 1);
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Metoda TransferTo(target, amount) istnieje (target: BankAccount, amount: liczba)")]
        public void S11_Method_TransferTo_Exists_WithSignature()
        {
            var m = GetNumericMethod("TransferTo", 2);
            m.GetParameters()[0].ParameterType.Should().Be(BankType, "pierwszy parametr powinien być typu BankAccount");
            IsNumeric(m.GetParameters()[1].ParameterType).Should().BeTrue("drugi parametr powinien być liczbowy (kwota)");
        }

        //Behaviour Tests

        [Trait("points", "1")]
        [Fact(DisplayName = "Deposit zwiększa Balance o kwotę")]
        public void B01_Deposit_Increases_Balance()
        {
            var account = CreateInstance();
            var balance = GetBalanceProp();
            SeedBalance(account, balance, 100m);

            var deposit = GetNumericMethod("Deposit", 1);
            var pType = deposit.GetParameters()[0].ParameterType;

            deposit.Invoke(account, new[] { ConvertTo(pType, 50m) });
            ToDecimal(balance.GetValue(account)).Should().Be(150m);
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Withdraw zmniejsza Balance o kwotę")]
        public void B02_Withdraw_Decreases_Balance()
        {
            var account = CreateInstance();
            var balance = GetBalanceProp();
            SeedBalance(account, balance, 200m);

            var withdraw = GetNumericMethod("Withdraw", 1);
            var pType = withdraw.GetParameters()[0].ParameterType;

            withdraw.Invoke(account, new[] { ConvertTo(pType, 30m) });
            ToDecimal(balance.GetValue(account)).Should().Be(170m);
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "TransferTo przenosi środki między kontami")]
        public void B03_TransferTo_Moves_Funds()
        {
            var a = CreateInstance();
            var b = CreateInstance();
            var balance = GetBalanceProp();

            SeedBalance(a, balance, 300m);
            SeedBalance(b, balance, 10m);

            var transfer = GetNumericMethod("TransferTo", 2);
            var amountType = transfer.GetParameters()[1].ParameterType;

            transfer.Invoke(a, new[] { b, ConvertTo(amountType, 70m) });

            ToDecimal(balance.GetValue(a)).Should().Be(230m);
            ToDecimal(balance.GetValue(b)).Should().Be(80m);
        }

        [Trait("points", "1")]
        [Fact(DisplayName = "Withdraw > Balance nie zmienia stanu konta")]
        public void B04_Withdraw_TooMuch_ShouldNotChangeBalance()
        {
            var account = CreateInstance();
            var balance = GetBalanceProp();
            SeedBalance(account, balance, 50m);

            var withdraw = GetNumericMethod("Withdraw", 1);
            var pType = withdraw.GetParameters()[0].ParameterType;

            var before = ToDecimal(balance.GetValue(account));
            try
            {
                withdraw.Invoke(account, new[] { ConvertTo(pType, 100m) });
            }
            catch { /* ignorujemy brak obsługi wyjątku */ }

            var after = ToDecimal(balance.GetValue(account));
            after.Should().Be(before, "nie powinno zejść poniżej 0 nawet bez obsługi wyjątku");
        }


    }
}