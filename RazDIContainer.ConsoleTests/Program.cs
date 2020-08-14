using System;

namespace RazDIContainer.ConsoleTests
{
    class Program
    {
        private static DIContainer container = new DIContainer();

        static void Main(string[] args)
        {
            container.Register<AnimalService, AnimalService>();
            container.Register<IAnimal, Cat>();
            container.RegisterSingleton<ICompany,Company>();

            var service = container.Resolve<AnimalService>();

            service.GetAnimal().MakeSound();

        }
    }

    public interface ICompany
    {
        string Name { get; set; }
    }

    public class Company : ICompany
    {
        public string Name { get; set; }
    }

    public interface IAnimal
    {
        void MakeSound();
    }

    public class AnimalService
    {
        private readonly IAnimal _animal;

        public AnimalService(IAnimal animal)
        {
            _animal = animal;
        }

        public IAnimal GetAnimal() => _animal;
    }

    public class Dog : IAnimal
    {
        public void MakeSound()
        {
            Console.WriteLine("Bark");
        }
    }

    public class Cat : IAnimal
    {
        public void MakeSound()
        {
            Console.WriteLine("Meow");
        }
    }
}
