[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/sHQkCuff)
﻿# Lab 1 — Klasy, pola, właściwości i metody (BankAccount)

## Cel zadania
Twoim zadaniem jest zaimplementowanie klasy **`BankAccount`** oraz przygotowanie krótkiego programu konsolowego, który tworzy kilka obiektów i demonstruje działanie metod na tych obiektach.

---

## Wymagania funkcjonalne

### 1) Implementacja klasy `BankAccount`
W klasie powinny znaleźć się **co najmniej**:
- **5 pól** (pole = *field*; mogą być prywatne),
- **5 właściwości** (*properties*), w tym obowiązkowe:
  - `AccountNumber`
  - `OwnerName`
  - `Balance`
  - `Currency`
  - `IsActive`
- **1 własne pole i 1 własna właściwość** (dowolne, przemyślane i uzasadnione).

**Wskazówki dotyczące walidacji:**
- Zastanów się, które elementy klasy wymagają walidacji (np. puste/`null` nazwy, waluta, dodatnie kwoty, itp.).
- Dopuszczalne jest zastosowanie prywatnych pól i publicznych właściwości z walidacją w `set`.

### 2) Metody obowiązkowe
- `Deposit(amount)` – zwiększa saldo konta,
- `Withdraw(amount)` – zmniejsza saldo konta,
- `TransferTo(target, amount)` – przelewa środki z jednego konta na inne (najpierw `Withdraw`, potem `Deposit`).

### 3) Dodatkowa metoda własna
Dodaj jedną, sensowną metodę własną, np. `Close()`, `Freeze()`, `ChangeOwner(...)`, itp.

### 4) Program demonstracyjny
W metodzie `Main`:
- utwórz **3 obiekty** klasy `BankAccount`,
- przetestuj wywołania metod (`Deposit`, `Withdraw`, `TransferTo`) oraz działanie własnej właściwości/metody,
- wypisz efekty na konsolę.

---

## Wymagania techniczne i organizacyjne

- **Namespace projektu**: `Lab1_Task.ConsoleApp`  
  (klasa powinna znajdować się w tym samym assembly, aby testy mogły ją odnaleźć).
- `Balance` **musi mieć publiczny getter**; setter może być prywatny (logika modyfikacji przez metody).
- Dopuszczalne różne konstruktory (bezparametrowy lub parametryzowany) – testy poradzą sobie z oboma.
- Typ kwoty może być `decimal/double/float/int/...` — testy konwertują kwoty automatycznie.
- Pola mogą być prywatne (testy zliczają również pola niepubliczne).

---

## Jak uruchomić lokalnie

1. Zaimplementuj klasę `BankAccount` w projekcie **`Lab1_Task.ConsoleApp`**.
2. (Opcjonalnie) dopisz prosty kod w `Program.cs`, który tworzy 3 konta i wykonuje operacje.
3. Uruchom testy:
   ```bash
   dotnet test
   ```

> Jeśli używasz Visual Studio, uruchom **Test Explorer** i uruchom wszystkie testy z projektu testowego.

---

## 🧪 Autograding — kryteria i punktacja

Testy są **ziarniste** – każdy test to **1 punkt** (atrybut `Trait("points","1")`).  
Łącznie w tej wersji zestawu (bez testów wyjątków) jest **15 punktów**.

### A. Struktura klasy (kontrakt publiczny)

| ID  | Opis testu                                                                                      | Punkty |
|------|--------------------------------------------------------------------------------------------------|---------|
| **S01** | Klasa `BankAccount` istnieje i jest publiczna. | 1 |
| **S02** | Właściwość `AccountNumber` istnieje (publiczna). | 1 |
| **S03** | Właściwość `OwnerName` istnieje (publiczna). | 1 |
| **S04** | Właściwość `Balance` istnieje i ma publiczny getter. | 1 |
| **S05** | Właściwość `Currency` istnieje (publiczna). | 1 |
| **S06** | Właściwość `IsActive` istnieje (publiczna). | 1 |
| **S07** | Klasa zawiera co najmniej **6 publicznych właściwości** (5 z zadania + 1 własna). | 1 |
| **S08** | Klasa zawiera co najmniej **5 pól** (w tym prywatne). | 1 |
| **S09** | Istnieje metoda `Deposit(amount)` – parametr typu liczbowego. | 1 |
| **S10** | Istnieje metoda `Withdraw(amount)` – parametr typu liczbowego. | 1 |
| **S11** | Istnieje metoda `TransferTo(target, amount)` – `target` to `BankAccount`, `amount` to liczba. | 1 |

---

### B. Zachowanie metod (testy funkcjonalne)

| ID  | Opis testu                                                                                      | Punkty |
|------|--------------------------------------------------------------------------------------------------|---------|
| **B01** | Metoda `Deposit` zwiększa wartość `Balance` o podaną kwotę. | 1 |
| **B02** | Metoda `Withdraw` zmniejsza wartość `Balance` o podaną kwotę. | 1 |
| **B03** | Metoda `TransferTo` poprawnie przenosi środki między kontami. | 1 |
| **B04** | W przypadku próby `Withdraw` większego niż stan konta, saldo nie ulega zmianie (nie spada poniżej zera). | 1 |

---

### 📊 Podsumowanie

- **Razem punktów:** 15  
- **Każdy test:** 1 pkt  
- Testy mają charakter **autogradingowy** — zaliczenie następuje po przejściu wszystkich testów.  
- W tej edycji **nie są testowane wyjątki** (`ArgumentOutOfRangeException`, `InvalidOperationException` itd.).  
- W przyszłych zadaniach wrócą testy walidacji i obsługi błędów.

---

## Czego testy **nie** wymagają (na tym etapie)
- Konkretnego formatu numeru konta.
- Twardo określonego typu dla `Balance` (może być `decimal` lub inny liczbowy).
- Twardo określonej listy walidacji (ale warto je mieć — ułatwią dalsze zadania).
- Obsługi wyjątków związanych z niepoprawnymi kwotami lub brakiem środków.

---

## Dobre praktyki (rekomendacje)
- Zaimplementuj **walidacje** tam, gdzie to ma sens (np. puste `OwnerName`, pusta `Currency`, ujemne kwoty).
- Trzymaj dane w **polach prywatnych** i wystawiaj **właściwości** z walidacją.
- Zadbaj o **czytelny interfejs publiczny** klasy – to on jest weryfikowany w testach.

---

## FAQ
- **Nie przechodzę testu „Balance (public getter)”** – sprawdź, czy `get` jest publiczny (setter może być prywatny).
- **Testy nie widzą klasy** – upewnij się, że namespace ustawiony w pliku BankAccount jest poprawny Lab1_Task.ConsoleApp
- **Ile właściwości łącznie?** – co najmniej **6** (5 z treści + 1 własna).
- **Ile pól?** – co najmniej **5** (mogą być prywatne).

Powodzenia! 💪
