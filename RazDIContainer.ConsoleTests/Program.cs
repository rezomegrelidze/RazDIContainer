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
            container.Register<IClock,SystemClock>();

            var service = container.Resolve<AnimalService>();

            service.GetAnimal().MakeSound();


            var clock = container.Resolve<IClock>();

            container.Register<ComplexService>();

            var complexService = container.Resolve<ComplexService>();
            complexService.MakeComplexAction();

            Console.WriteLine(clock.Time);
        }
    }

    public class ComplexService
    {
        private readonly IClock _clock;
        private readonly ICompany _company;
        private readonly IAnimal _animal;

        public ComplexService(IClock clock,ICompany company,IAnimal animal)
        {
            _clock = clock;
            _company = company;
            _company.Name = "Microsoft";
            _animal = animal;
        }

        public void MakeComplexAction()
        {
            var time = _clock.Time;
            var companyName = _company.Name;
            Console.WriteLine($"Time: {time}, companyName: {companyName}");
            _animal.MakeSound();
        }
    }

    public interface IClock
    {
        public DateTime Time { get; }
    }

    public class SystemClock : IClock
    {
        public DateTime Time => DateTime.Now;
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
