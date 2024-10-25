using System;
using System.Collections.Generic;
namespace BookStoreApp
{
    public struct Tsena
    {
        public decimal Rub { get; set; }
        public decimal Usd { get; set; }
    }
    public class NetNaSkladeException : Exception
    {
        public NetNaSkladeException(string msg) : base(msg) { }
    }
    public interface IMagazin
    {
        void Dobavit(Kniga k);
        Kniga Iskat(string name);
        void Kupit(string name);
    }
    public class Kniga
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public Tsena Price { get; set; }
        public int Qty { get; set; }
        public event Action<string> LowStock;
        public void Pokupka()
        {
            if (Qty <= 0)
                throw new NetNaSkladeException($"Нет книги '{Name}' на складе");

            Qty--;
            if (Qty < 5) LowStock?.Invoke(Name);
        }
    }
    public class Magazin : IMagazin
    {
        private List<Kniga> knigi = new List<Kniga>();
        public void Dobavit(Kniga k) { knigi.Add(k); }
        public Kniga Iskat(string name) => knigi.Find(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        public void Kupit(string name)
        {
            var k = Iskat(name);
            if (k != null)
            {
                try
                {
                    k.Pokupka();
                    Console.WriteLine($"Купили '{k.Name}'");
                }
                catch (NetNaSkladeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Книга '{name}' не найдена");
            }
        }
    }
    class Program
    {
        static void Main()
        {
            var magazin = new Magazin();

            var kniga1 = new Kniga
            {
                Name = "Clear Code",
                Author = "Adis Azamatov",
                Genre = "Coding",
                Price = new Tsena { Rub = 600, Usd = 8 },
                Qty = 10
            };

            kniga1.LowStock += name => Console.WriteLine($"Внимание: '{name}' - мало на складе!");

            magazin.Dobavit(kniga1);

            magazin.Kupit("Clear Code");
            magazin.Kupit("Гарри Поттер");
        }
    }
}
