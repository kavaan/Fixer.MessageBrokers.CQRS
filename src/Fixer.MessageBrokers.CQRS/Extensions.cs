using System.Threading.Tasks;
using Fixer.CQRS.Commands;
using Fixer.CQRS.Events;
using Fixer.MessageBrokers.CQRS.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
namespace Fixer.MessageBrokers.CQRS
{
    public static class Extensions
    {
        public static Task SendAsync<TCommand>(this IBusPublisher busPublisher, TCommand command, object context)
            where TCommand : class
            => busPublisher.PublishAsync(command, context: context);

        public static Task PublishAsync<TEvent>(this IBusPublisher busPublisher, TEvent @event, object context)
            where TEvent : class, IEvent
            => busPublisher.PublishAsync(@event, context: context);

        public static IBusSubscriber SubscribeCommand<T>(this IBusSubscriber busSubscriber) where T : class, ICommand
            => busSubscriber.Subscribe<T>((sp, command, ctx) => sp.GetService<ICommandHandler<T>>().HandleAsync(command));

        public static IBusSubscriber SubscribeEvent<T>(this IBusSubscriber busSubscriber) where T : class, IEvent
            => busSubscriber.Subscribe<T>((sp, @event, ctx) => sp.GetService<IEventHandler<T>>().HandleAsync(@event));

        public static IFixerBuilder AddServiceBusCommandDispatcher(this IFixerBuilder builder)
        {
            builder.Services.AddTransient<ICommandDispatcher, ServiceBusMessageDispatcher>();
            return builder;
        }

        public static IFixerBuilder AddServiceBusEventDispatcher(this IFixerBuilder builder)
        {
            builder.Services.AddTransient<IEventDispatcher, ServiceBusMessageDispatcher>();
            return builder;
        }
    }
}
