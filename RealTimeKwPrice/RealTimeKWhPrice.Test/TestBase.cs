using Domain.Interfaces;
using FakeItEasy;
using MediatR;

namespace RealTimeKWhPrice.Test
{
    public abstract class TestBase<T> where T : class
    {
        protected readonly IMediator Mediator;
        protected readonly IGenericRepository<T> _Db;
        protected TestBase()
        {
            Mediator = A.Fake<IMediator>();
            _Db = A.Fake<IGenericRepository<T>>();
        }
    }
}
